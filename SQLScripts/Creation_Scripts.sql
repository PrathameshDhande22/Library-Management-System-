-- creation of database
create database LibraryDB
go

-- use the created database
use LibraryDB
go

-- creation of Category Table
create table Category (
	categoryid int primary key identity,
	categoryname nvarchar(50)
)
go

/* Creation of Constraints */

-- unique constraint of category
alter table category
add Constraint UQ_Category_Categoryname unique(categoryname)
go

-- Creation of Roles Table
create table Roles (
	roleid int primary key identity,
	rolename nvarchar(40)
)
go

-- Creation of library setting table
create table Settings (
	settingId int primary key identity,
	maxissueDays int,
	dailyfineamount int,
	maxbookperuser int,
	updatedon datetime
)
go

-- creation of Users Tables
create table Users (
	id int primary key identity,
	username nvarchar(80) not null,
	[password] nvarchar(80) not null,
	firstname nvarchar(80) not null,
	lastname nvarchar(80) not null,
	gender nvarchar(30) not null,
	email nvarchar(90) not null,
	[address] nvarchar(max) not null,
	pincode int not null,
	[state] nvarchar(100) not null,
	city nvarchar(80) not null,
	phoneno nvarchar(20) not null,
	createdon datetime not null,
	updatedon datetime null,
	updatedby int null,
	deletedon datetime null,
	isdeleted bit not null
)
go

/* creation of constraints in user table */

-- default constraint on user table for createdon column 
alter table users
add constraint DF_Users_createdon default getdate() for createdon
go

-- default constraint on user table for isdeleted column
alter table users
add constraint DF_Users_isdeleted default 0 for isdeleted
go

-- foreign key constraint on users table, updatedby column
alter table users
add constraint FK_Users_updatedby foreign key (updatedby) references users(id) on delete no action
go

-- unique key constraint on users table, username and email column
alter table users
add constraint UQ_Users_username unique(username)
go

alter table users
add constraint UQ_Users_email unique(email)
go

-- creation of roles mapping table
create table RoleMapping (	
	id int primary key identity,
	roleid int not null,
	userid int not null,
)
go

/* Creation of Constraints on RoleMapping Table */

-- foreign key constraint on role mapping table on role id
alter table RoleMapping
add constraint FK_RoleMapping_roleid foreign key(roleid) references Roles(roleid) on delete cascade
go

-- foreign key constraint on role mapping table on user id
alter table RoleMapping
add constraint FK_RoleMapping_userid foreign key(userid) references Users(id) on delete cascade
go

-- creation of Books table
create table Books (
	bookid int primary key identity,
	categoryid int not null,
	title nvarchar(60) not null,
	isbn nvarchar(20) not null,
	authorname nvarchar(70) not null,
	coverimage nvarchar(max) not null,
	publicationYear int not null,
	availableQty int not null,
	originalQty int not null,
	createdon datetime not null,
	createdby int not null,
	updatedon datetime null,
	updatedby int null,
	isdeleted bit not null,
	deletedon datetime null,
	deletedby int null
)
go

/* Creating Constraints on Books Table */

-- foreign key constraints on category id column
alter table books
add constraint FK_Books_categoryid foreign key (categoryid) references Category(categoryid)
go

-- foreign key constraints on createdby column
alter table books
add constraint FK_Books_CreatedBy foreign key (createdby) references Users(id)
go

-- foreign key constraints on updatedby column
alter table books
add constraint FK_Books_UpdatedBy foreign key (updatedby) references Users(id) on delete no action
go

-- foreign key constraints on deletedby column
alter table books
add constraint FK_Books_DeletedBy foreign key(deletedby) references Users(id) on delete no action
go

-- defualt constraints on createdon column
alter table books
add constraint DF_Books_CreatedOn Default getdate() for createdon
go

-- default constraints on isdeleted column
alter table books
add constraint DF_Books_isDeleted Default 0 for isdeleted
go

-- creation of issues table
create table Issues(
	id int primary key identity,
	userid int not null,
	bookid int not null,
	issuedate datetime not null,
	duedate datetime not null,
	[status] nvarchar(40) not null,
	returneddate datetime null,
	fineamount int null,
	dailyfineamount int not null
)
go

/* Creating the constraints for Issues table */

-- foreign key constraint for userid column
alter table issues
add constraint FK_Issues_Userid foreign key(userid) references users(id) on delete cascade
go

-- foreign key constraint for bookid column
alter table issues
add constraint FK_Issues_Bookid foreign key(bookid) references books(bookid) on delete cascade
go

-- default constraint for issuedate column
alter table issues
add constraint DF_Issues_Issuedate default getdate() for issuedate 
go

-- adding the new column for who returned the book
alter table Issues
add returnedby int null

-- foreign key constraint for who returned the book
alter table issues
add constraint FK_Issues_ReturnedBy foreign key (returnedby) references Users(id)

-- creating the table for catching the errors
create table ErrorLog (
	id int primary key identity,
	error nvarchar(max),
	errormsg nvarchar(max),
	errorline int,
	errorproc nvarchar(50)
)
go

/* Views Creation */

-- views for getting the users with their login details along with roles
create view vw_LoginDetails
as
	select u.id,u.username,u.password,u.email,r.rolename from Users as u
	join RoleMapping as m
	on m.userid=u.id
	join Roles as r
	on m.roleid=r.roleid
	where u.isdeleted=0
go

/* Stored Procedure */

-- ========================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description: stored procedure for registering the user by passing the required Parameters to it.
-- ========================================================================================
create proc spRegisterUser
@username nvarchar(80),
@password nvarchar(80),
@firstname nvarchar(80),
@lastname nvarchar(80),
@gender nvarchar(30),
@email nvarchar(90),
@address nvarchar(max),
@pincode int,
@state nvarchar(100),
@city nvarchar(80),
@phoneno nvarchar(20),
@issuccess bit out
as
begin
	begin try
		begin transaction
			-- insert the data into the users table
			insert into Users(username,password,firstname,lastname,gender,email,address,pincode,state,city,phoneno) values
			(@username,@password,@firstname,@lastname,@gender,@email,@address,@pincode,@state,@city,@phoneno)

			-- insert its role as users
			insert into RoleMapping(userid,roleid) values
			(IDENT_CURRENT('Users'),2)
			set @issuccess=1
			commit transaction
	end try
	begin catch
		rollback transaction
		insert into ErrorLog(error,errorline,errormsg,errorproc) values 
		(ERROR_STATE(),ERROR_LINE(),ERROR_MESSAGE(),ERROR_PROCEDURE())
		set @issuccess=0
	end catch
end
go

-- ===============================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Getting the Logging the user by passing the Username and Password.
-- ==============================================================================
create proc spLoginUser
@username nvarchar(80),
@password nvarchar(80)
as
begin
	select top 1 id,username, password from users 
	where (username=@username or email=@username) and password=@password and isdeleted=0
end
go

-- ================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	stored Procedure for getting the usernames or email for new user as well as loggedin users
-- ==================================================================================
create proc spGetUserNameOrEmail
@username nvarchar(80)=null,
@useremail nvarchar(80)=null,
@loginid int=0
as
begin	
	select 
        case when exists (select * from users where username = @username and (@loginid = 0 or id != @loginid))
            then 1 else 0 end as isusernametaken,
        case when exists (select * from users where email = @useremail and (@loginid = 0 or id != @loginid))
            then 1 else 0 end as isemailtaken

end
go

-- =================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Updating the user Profile by passing the required parameters to it also checks for the role.
-- ================================================================================
create proc spUpdateUserProfile
@id int,
@username nvarchar(80),
@password nvarchar(80)=null,
@firstname nvarchar(80),
@lastname nvarchar(80),
@gender nvarchar(30),
@email nvarchar(90),
@address nvarchar(max),
@pincode int=0,
@state nvarchar(100),
@city nvarchar(80),
@phoneno nvarchar(20),
@userid int=0,
@rolename nvarchar(40)=null,
@issuccess bit out
as
begin
	begin try
		begin transaction
				-- for updating the user profile
				-- update the user first
				update users set username=@username,firstname=@firstname,lastname=@lastname,
					password=iif(@password is null,password,@password)		,gender=@gender,email=@email,address=@address,pincode=@pincode,state=@state,city=@city,phoneno=@phoneno,updatedon=GETDATE(),updatedby=iif(@userid=0,null,@userid)
				where id=@id and isdeleted=0
				set @issuccess=1

				-- update the roles of that user if rolename is passed then admin is trying to update the role
				if(@rolename is not null)
					begin
						-- before updating the role from user to lib/admin if user has issued books then return that books
						declare @role nvarchar(40)
						select @role=r.rolename from RoleMapping as rm
						join Roles as r
						on rm.roleid=r.roleid
						where userid=@id
						
						if(@role!=@rolename)
							begin
								-- update the books available quantity to +1
								update Books 
								set availableQty=availableQty+1
								where bookid in (select bookid from Issues where userid=@id and status='Issued')

								-- return all the books which the user has issued
								update Issues
								set [status]=case 
												when datediff(day,duedate,getdate()) > 0 then 'LateReturn'
												when datediff(day,duedate,getdate()) <=0 then 'Return'
								end,
								returneddate=GETDATE(),
								fineamount=case 
												when DATEDIFF(day,duedate,getdate()) >0 then dailyfineamount*DATEDIFF(day,duedate,getdate())
												when DATEDIFF(day,duedate,getdate()) <=0 then 0
											end
								where userid=@id and status='Issued'

								-- update the role
								update RoleMapping set 
								roleid=(select roleid from Roles where rolename=@rolename) 
								where userid=@id
								set @issuccess=1
							end
					end
		commit transaction
	end try
	begin catch
		set @issuccess=0
		rollback transaction
	end catch
end
go

-- =========================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Updating the Password for the user id also verifying if the oldpassword matches to update.
-- =========================================================================================
create proc spChangePassword
@id int,
@password nvarchar(80)=null,
@oldpassword nvarchar(80)=null,
@issuccess bit out
as
begin
	begin try
		-- for updating the password
		update users set password=@password 
		where password=@oldpassword and id=@id and isdeleted=0
		set @issuccess=1
	end try
	begin catch
		set @issuccess=0
	end catch
end
go

-- ======================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	getting the roles for the particular loggedin user
-- =======================================================================================
create proc spGetUsersRole
@id int
as
begin
	select r.rolename from users as u
	join RoleMapping as rm
	on u.id=rm.userid
	join Roles as r
	on rm.roleid=r.roleid
	where u.id=@id and u.isdeleted=0
end
go


-- =================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	stored procedure for fetching the library setting based on passed Setting ID and also for updating
-- =================================================================================
create proc spLibrarySetting
@maxissuedays int=null,
@dailyfineamount int=null,
@maxbookperuser int=null,
@settingid int,
@issuccess bit out
as
begin
	if(@maxissuedays is null and @dailyfineamount is null and @maxbookperuser is null)
		begin
			-- fetch the library setting
			set @issuccess=1
			select * from Settings where settingId=@settingid
		end
	else if(@maxissuedays is not null and @dailyfineamount is not null and @maxbookperuser is not null)
		begin
			-- update the library setting
			update settings set maxbookperuser=@maxbookperuser,dailyfineamount=@dailyfineamount,maxissueDays=@maxissuedays,updatedon=GETDATE()
			where settingId=@settingid
			set @issuccess=1
		end
	else
		insert into ErrorLog(error,errorline,errormsg,errorproc) values 
		('Failedtoupdate',null,'Failed to update pass the required parameters','spLibrarySetting')
		set @issuccess=0
end
go

-- ========================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	For Getting the Users but not the currently login admin user or can get the single user by passing the User id.
-- =========================================================================================
create proc spGetUsersOrSingle
@userid int=0
as
begin
			select u.id,u.username,u.password,u.firstname,u.lastname,u.gender,u.email,u.address,u.pincode,u.state,u.city,u.phoneno,u.createdon,u.updatedon,u.updatedby,u.deletedon,u.isdeleted,rm.id roleid,r.rolename 
			from users as u
			join RoleMapping as rm
			on u.id=rm.userid
			join Roles as r
			on rm.roleid=r.roleid
			where (@userid=0 or (u.id=@userid and u.isdeleted=0))
			order by u.createdon desc
end
go

-- =====================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Stored procedure for deleting the User by passing the User id for which you want to delete
-- =====================================================================================
create proc spDeleteUser
@id int,
@issuccess bit=0 out
as
begin
	begin try
		begin transaction
			-- checking if the user has issued the books or not
			declare @issuecount int
			select @issuecount=count(id) from Issues
			where userid=@id and status='Issued'

			if(@issuecount=0)
				begin
					-- update the users and mark as deleted
					update users set isdeleted=1,deletedon=GETDATE() where id=@id
					set @issuccess=1
				end
			else
				set @issuccess=0
		commit transaction
	end try
	begin catch
		set @issuccess=0
		rollback transaction
	end catch
end
go
	
-- ==============================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Stored Procedure for creating the Category for the passed categoryname and returning its id otherwise list of category.
-- =============================================================================
create proc spCreateOrGetCategory
@categoryname nvarchar(50)=null,
@issuccess bit out,
@insertedid int=0 out
as
begin
	begin try
		if(@categoryname is not null)
			begin
				-- insert the category
				insert into Category(categoryname) values (@categoryname)
				set @issuccess=1
				set @insertedid=SCOPE_IDENTITY()
			end
		else if(@categoryname is null)
			begin
				-- give all the category name
				select categoryid,categoryname from Category
				order by categoryid asc
				set @issuccess=1 
				set @insertedid=0
			end
	end try
	begin catch
		set @issuccess=0
		set @insertedid=0
	end catch
end
go

-- =============================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Creating and Upating the Book Details.
-- =============================================
create proc spCreateUpdateBook
@title nvarchar(60),
@isbn nvarchar(20),
@categoryid int,
@authorname nvarchar(70),
@coverimage nvarchar(max),
@publicationyear int,
@originalQty int=0,
@availableQty int=0,
@updatedby int=0,
@createdby int=0,
@toinsert bit=0,
@bookid int=0,
@issuccess bit out
as
begin	
	begin try
		begin transaction
			if(@toinsert =1 and @originalQty!=0)
				begin
					-- insert the data into the book table
					insert into Books (title,isbn,authorname,coverimage,publicationYear,availableQty,originalQty,createdby,categoryid)
					values (@title,@isbn,@authorname,@coverimage,@publicationyear,@originalQty,@originalQty,@createdby,@categoryid)
					set @issuccess=1
				end
			else if(@toinsert = 0)
				begin
					-- update the book details
					if(@bookid!=0 and @availableQty!=0)
						begin
							update books set										title=@title,categoryid=@categoryid,isbn=@isbn,authorname=@authorname,coverimage=@coverimage,publicationYear=@publicationyear,
							availableQty=@availableQty
							,originalQty=case when availableQty<=@availableQty
													then originalQty+(@availableQty-availableQty)
												when availableQty>@availableQty
													then originalQty-(availableQty-@availableQty)
										end
							,updatedon=GETDATE(),updatedby=@updatedby
							where bookid=@bookid
							set @issuccess=1
						end
					else
						set @issuccess=0
				end
			else 
				set @issuccess=0
			commit transaction
	end try
	begin catch
		rollback transaction
		insert into ErrorLog(error,errormsg,errorline,errorproc) values
		('Failed to insert the data into books table',@title,null,'spCreateUpdateBooks')
		set @issuccess=0
	end catch
end
go

-- ================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Getting the Book Details along with Issue of the user Details if book id is passed otherwise returns the List of Books.
-- ================================================================================
create proc spGetBooks
@bookid int =0,
@userid int =0,
@pageno int=0,
@pagesize int=0,
@sort int=0,
@title nvarchar(60)=null,
@authorname nvarchar(70)=null,
@category int=0
as
begin
			set @title=IIF(@title is null,'%%','%'+@title+'%');
			set @authorname=IIF(@authorname is null,'%%','%'+@authorname+'%');
			

			-- fetching total records present in the book
			declare @totalrecords int,@filteredrecords int;
			select @totalrecords=count(*) from Books where isdeleted=0;

			-- fetching the book details
			with fetchedRecords as
				(
					select b.bookid,b.title,b.isbn,b.authorname,b.coverimage,b.publicationYear,b.availableQty,b.originalQty,b.createdon,b.createdby,b.updatedon,b.updatedby,b.isdeleted,b.deletedon,b.deletedby,
					case 
						when i.bookid is not null and i.userid = @userid then 1
						else 0
					end as isIssued
					,c.categoryid,c.categoryname,i.id,i.userid,i.issuedate,i.duedate,i.status,i.returneddate,i.fineamount,i.dailyfineamount,i.returnedby
					from books as b
					join Category as c
					on c.categoryid=b.categoryid
					left join Issues as i
					on i.bookid=b.bookid and i.userid=@userid and i.status='Issued'
					where ((@bookid = 0 AND b.isdeleted = 0) OR b.bookid = @bookid)
					and b.title like @title 
					and b.authorname like @authorname 
					and (@category=0 or b.categoryid=@category)
				)
			
			-- fetching the total filtered records
			select @totalrecords as TotalRecords,count(fr.bookid) over() as TotalFilteredRecords,fr.* 
			from fetchedRecords as fr
			order by 
				case @sort
						-- latest books
						when 0 then createdon 
				end desc,
				case @sort 
						-- by a -z
						when 1 then title
				end asc,
				case @sort
					-- by z-a
					when 2 then title
				end desc,
				case @sort
					-- newest added
					when 3 then createdon
				end desc,
				case @sort
					-- oldest added
					when 4 then createdon
				end asc,
				case @sort	
					-- publication oldest
					when 5 then publicationYear
				end desc,
				case @sort
					-- publication newest
					when 6 then publicationYear
				end asc
				offset @pageno*@pagesize rows
				fetch next @pagesize rows only;
end
go

-- ==============================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Deleting the Book also marking deletedby as the userid.
-- =================================================================
create proc spDeleteBook
@bookid int,
@userid int,
@issuccess bit output
as
begin
		if(@bookid!=0 and @userid!=0)
			begin
				-- check if the books are currently issued by users
				declare @issuecount int
				select @issuecount=count(*) from Issues where bookid=@bookid and status='Issued'

				if(@issuecount!=0)
					begin
						set @issuccess=0
						raiserror('Book is currently issued by users',15,1)
					end
				else
					begin
						-- when issue count is zero then allow to delete the book
						update books set 
						isdeleted=1,deletedon=GETDATE(),deletedby=@userid 
						where bookid=@bookid
						set @issuccess=1
					end
			end
		else
			set @issuccess=0
end
go

-- ========================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	issuing the book for the passed BookId and userid, outputs the errormsg if the request is not valid.
-- ========================================================================
create proc spIssueBook
@bookid int,
@userid int,
@returnerid int=0,
@errormsg nvarchar(max) out,
@issuccess bit out
as
begin
	begin try
		begin transaction
					-- fetching the library setting
					declare @maxbook int, @dailyfineamount int, @maxdays int
					select 
					@maxbook=maxbookperuser,
					@dailyfineamount=dailyfineamount,
					@maxdays=maxissueDays 
					from Settings where settingId=1
				
					-- checking if the user has already issued these book or not
					declare @issuedBookbyuser int
					declare @BookStatus nvarchar(60);

					with issuebooks
					as
						(select * from Issues
						where userid=@userid and status='Issued')
				
					select
					@issuedBookbyuser = count(*),  
					@BookStatus = case 
									 when exists (select 1 
												  from issuebooks 
												  where bookid = @bookid and 
												  userid=@userid and status='Issued') 
									 then 'Issued' 
									 else 'NotIssued' 
									end  
					from issuebooks

					-- if book is already issued and not reach the max limit issue the book
					if(@BookStatus!='Issued' and @issuedBookbyuser!=@maxbook)
						begin

							-- check if the quantity is available to issue
							declare @avlQty int
							select @avlQty=availableQty from Books where bookid=@bookid and isdeleted=0

							if(@avlQty>=1)
								begin
									-- insert into the issues table
									insert into Issues(userid,bookid,duedate,status,dailyfineamount)
									values 
									(@userid,@bookid,DATEADD(day,@maxdays,GETDATE()),'Issued',@dailyfineamount)

									-- decrease available quantity by one in books table
									update Books set availableQty=availableQty-1
									where bookid=@bookid and isdeleted=0

									set @issuccess=1
									set @errormsg='Successfully Issued the Book'
								end
							else if(@avlQty<=0)
								begin
									set @issuccess=0
									set @errormsg='Book is currently Out of Stock'
								end
							else
								begin
									set @issuccess=0
									set @errormsg='Book Do not exist'
								end
						end
					else if(@BookStatus='Issued')
						begin
							set @issuccess=0
							set @errormsg='Book is Already Issued by User'
						end
					else if(@issuedBookbyuser>=@maxbook)
						begin
							set @issuccess=0
							set @errormsg='Max Book Issue Limit Reach Return one or More Book'
						end
					else
						begin
							set @issuccess=0
							set @errormsg='Some Error Happened At Our End'
						end
			commit transaction
	end try
	begin catch
		insert into ErrorLog(error,errorline,errormsg,errorproc) values 
		(ERROR_STATE(),ERROR_LINE(),ERROR_MESSAGE(),ERROR_PROCEDURE())
		rollback transaction
		set @errormsg='Error at our end'
		set @issuccess=0
	end catch
end
go

-- =======================================================================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Returning the Books based on Userid and bookid outputs the Errormsg if any error message occurs.
-- ========================================================================================
create proc spReturnBook
@userid int,
@returnerid int=0,
@bookid int,
@errormsg nvarchar(max) out,
@issuccess bit out
as
begin
	begin try
		begin transaction
			-- checking if the book id is issued by user
			declare @countissue int,@duedate datetime
			select @countissue=count(*),@duedate=max(duedate)
			from Issues
			where userid=@userid and bookid=@bookid and status='Issued'

			if(@countissue=1)
				begin
					-- checking if the book has passed due date
					declare @daydiff int=DATEDIFF(day,@duedate,GETDATE())

					-- update the issues table
					update Issues set status=case 
											when @daydiff>0 then 'LateReturn'
											when @daydiff<=0 then 'Return'
											end,
					fineamount= case 
								when @daydiff>0 then dailyfineamount*@daydiff
								else null
								end,
					returneddate=GETDATE(),
					returnedby=iif(@returnerid=0,null,@returnerid)
					where userid=@userid and bookid=@bookid

					-- update the available quantity to +1
					update books set availableQty=availableQty+1 
					where bookid=@bookid

					set @issuccess=1
					set @errormsg=case when @daydiff>0 then 'Successfully Paid the Fine and returned the book'
										else 'Successfully returned the book'
									end
				end
			else
				begin
					set @issuccess=0
					set @errormsg='You have not issued these book'
				end
		commit transaction
	end try
	begin catch
		insert into ErrorLog(error,errorline,errormsg,errorproc) values 
		(ERROR_STATE(),ERROR_LINE(),ERROR_MESSAGE(),ERROR_PROCEDURE())
		rollback transaction
		set @errormsg='Error at our end'
		set @issuccess=0
	end catch
end
go

-- =============================================
-- Author:		Prathamesh
-- Create date: 18/10/24
-- Description:	Showing all the details of the Issues along with filtering support and pagination.
-- =============================================
create proc spDashBoard
@status nvarchar(50)=null,
@title nvarchar(50)=null,
@name nvarchar(50)=null,
@pageno int=0,
@userid int=0,
@totalFine int=0 out,
@totalIssued int=0 out
as
begin
	-- fetching the total fine
	select @totalFine=case when sum(fineamount) is null then 0
						else sum(fineamount)
					end
	from Issues 
	where 
	status='LateReturn' and (@userid=0 or userid=@userid)

	-- fetching the total issued books by user or overall
	select @totalIssued=count(id) 
	from Issues
	where 
	status='Issued' and (@userid=0 or userid=@userid);
	
	-- fetching all the issue details along with its status
	with issuedetails as
	(
		select i.id,i.userid,i.bookid,i.issuedate,i.duedate,case 
															when status='Issued' and DATEDIFF(day,duedate,GETDATE())>0  then 'DuePassed'
															else status
														end as status,
			i.returneddate,i.fineamount,i.dailyfineamount,b.title,CONCAT(u.firstname,' ',u.lastname) as IssuedName,iif(ru.username is null,null,CONCAT(ru.firstname,' ',ru.lastname)) as ReturnedName
		from Issues as i
		join Books as b
		on i.bookid=b.bookid
		join Users as u
		on u.id=i.userid
		left join Users as ru
		on i.returnedby=ru.id
		where (@userid=0 or i.userid=@userid)
	)

	-- applying the pagination and filter on the fetched issuedetails.
	select i.*,count(i.id) over() as TotalRecords 
	from issuedetails as i
	where 
	(@status is null or i.status=@status) 
	and (@title is null or i.title like '%'+@title+'%')
	and (@name is null or i.IssuedName like '%'+@name+'%')
	order by i.status,
		case @status
			-- when status is else then by issued date
			when 'Issued' then i.issuedate
		end desc,
		case @status
			-- when status is Returned then by returned date
			when 'Return' then i.returneddate
		end desc,
		case @status
			-- when status is late return then by returned date
			when 'LateReturn' then i.returneddate
			else i.issuedate
		end desc
	offset @pageno*10 rows
	fetch next 10 rows only
end
go



/*********************** Insertion Scripts Required to Run the Database **********************/

insert into Users(username,password,firstname,lastname,gender,email,address,pincode,state,city,phoneno) 
values 
('admin','admin@123','Prathamesh','Dhande','Male','admin123@gmail.com','Boisar',202332,'Maharashtra','Boisar','997022298')
go

/* Insert Data in Roles */
insert into Roles(rolename) values 
('Admin'),
('User'),
('Librarian')
go

/* Giving the access to the role of admin */
insert into RoleMapping(roleid,userid) values (1,1)
go

-- inserting the library setting
insert into Settings(maxissueDays,dailyfineamount,maxbookperuser) values
(10,50,5)
go

-- inserting the category records
INSERT INTO Category (CategoryName)
VALUES 
('Fiction'),
('Non-Fiction'),
('Science'),
('Biography'),
('Fantasy'),
('History'),
('Technology')
go