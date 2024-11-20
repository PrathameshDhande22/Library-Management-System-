/* ********************************************

Select Scripts : These Scripts are for Viewing the Data Purposes only

***********************************************/

/* Select query for users table */
select * from Users

/* Select Query for Roles table */
select * from Roles

/* Select Query for RoleMapping Table */
select * from RoleMapping

/* Select Query with join for getting the details for admin with user name and password */
select u.id,u.username,u.password,u.email,r.rolename from Users as u
join RoleMapping as m
on m.userid=u.id
join Roles as r
on m.roleid=r.roleid

/* Select query for Error Tables */
select * from ErrorLog

-- using view of logindetails
select * from vw_LoginDetails

/* Select query for setting tables */
select * from Settings

-- select query for login details either from username or email
select top 1 id,username,password from (
	select id,username,password from users
	where username='user@gmail.com' or email='user@gmail.com'
) as logindetails
where password='User@12'

select top 1 id,username, password from users
where (username='user@gmail.com' or email='user@gmail.com')
and password='user@12'

-- for fetching the password for username and id
select password from users
where password='Tester!' and username='Tester' and id=7

-- for updating the password
update users set password='Tester!1' where password='Tester@12' and username='tester12' and id=8

exec spLoginUser 'tester@gmail.com','Tester!2'
exec spUserProfile 'Tester',7

select username from users where (username='mark_jones' or email='admin') and (username!='mark_jones' and email!='mark.jones@example.com')

exec spGetUserNameOrEmail @username=N'User34',@useremail=NULL,@loginid=7

exec spUpdateProfileOrPassword 4,'Teste','Tester@12','Tester@123' -- return one row affected

select username from users where username='mark.jones@example.com' or email='mark.jones@xample.com'

/* Select query for fetching the role of the particular login user */
select r.rolename from users as u
join RoleMapping as rm
on u.id=rm.userid
join Roles as r
on rm.roleid=r.roleid
where u.id=5

exec spGetUsersRole 1,'admin'

exec spLibrarySetting 10,23,6,1, @issuccess output
select @issuccess
declare @issuccess int 
exec spLibrarySetting null,null,null,1,@issuccess output
select @issuccess

/* select query to get the user details along with its rolename */
select u.id,u.username,u.firstname,u.lastname,u.gender,u.email,u.isdeleted,rm.id,r.rolename 
from users as u
join RoleMapping as rm
on u.id=rm.userid
join Roles as r
on rm.roleid=r.roleid
where u.id!=1

/* Select Query to get the single user details along with its rolename and its last updated by user */
select u.id,u.username,u.password,u.firstname,u.lastname,u.gender,u.email,u.address,u.pincode,u.state,u.city,u.phoneno,u.createdon,u.updatedon,u.updatedby,u.deletedon,u.isdeleted,uu.id updaterid,uu.username,uu.password,uu.firstname,uu.lastname,uu.gender,uu.email,uu.address,uu.pincode,uu.state,uu.city,uu.phoneno,uu.createdon,uu.updatedon,uu.updatedby,uu.deletedon,uu.isdeleted,rm.id roleid,r.rolename 
from users as u
join RoleMapping as rm
on u.id=rm.userid
join Roles as r
on rm.roleid=r.roleid
left join Users as uu
on uu.id=u.updatedby
where u.id=3 and u.id!=1

-- same using stored proc
exec spGetUsersOrSingle 1

/* Select Query for getting the Category */
select * from Category

exec spDeleteUser 14

exec spGetUserNameOrEmail 'mark_jones',@loginid=4

update RoleMapping set 
roleid=(select roleid from Roles where rolename='Admin') 
where userid=12
select * from vw_LoginDetails where id=12

declare @issuccess1 int
declare @categoryid int
exec spCreateorgetCategory 'Horr122ddedfd',@issuccess1 output,@categoryid output
select @categoryid as successer
select @issuccess1

select IDENT_CURRENT('category')

/* Select Query for fetching the total records of books */
select * from Books

/* Select Query for fetching the total records of books with its user name */
declare @bookid int=1
select 
	b.bookid,b.title,b.isbn,b.authorname,b.coverimage,b.publicationYear,b.availableQty,b.originalQty,b.createdon,b.createdby,b.updatedon,b.updatedby,b.isdeleted,b.deletedon,b.deletedby,cu.username createdusername,uu.username updatedusername,du.username deletedusername,c.categoryid,c.categoryname
from books as b
join Category as c
on c.categoryid=b.categoryid
left join Users as uu
on uu.id=b.updatedby
left join Users as du
on du.id=b.deletedby
left join Users as cu
on cu.id=b.createdby
where (@bookid=0 or b.bookid=@bookid)

select getDate()

update books set isdeleted=0,deletedon=NULL where bookid in (2)
update Users set isdeleted=0,deletedon=NULL where id=6

-- same using sp
exec spGetBooks

/* Select Query for Issues Table */
select * from Issues

-- Select Query for getting the issue details with book details
declare @id int=2
select i.*,b.*,uub.username as updatedusername,cu.username as createdusername,
cat.*
from Issues as i
join Books as b
on i.bookid=b.bookid
left join Users as uub
on b.updatedby=uub.id
left join Users as cu
on b.createdby=cu.id
join Category as cat
on b.categoryid=cat.categoryid

-- select query for getting the issue details based on user id
select i.* from Issues as i
join Books as b
on i.bookid=b.bookid
join Users as u
on i.userid=u.id
join RoleMapping as rm
on u.id=rm.userid
join Roles as r
on rm.roleid=r.roleid
where r.roleid=(select roleid from Roles where rolename='User')
and i.userid=4 and i.bookid=1 and i.status='Issued'

-- same using sp
exec spGetBooks @bookid=15,@userid=7,@pageno=0,@pagesize=12,@sort=0,@title=NULL,@authorname=NULL,@category=0


declare @bookid1 int=0
select b.bookid,b.title,b.isbn,b.authorname,b.coverimage,b.publicationYear,b.availableQty,b.originalQty,b.createdon,b.createdby,b.updatedon,b.updatedby,b.isdeleted,b.deletedon,b.deletedby,cu.username createdusername,uu.username updatedusername,du.username deletedusername,
CASE 
        WHEN i.bookid IS NOT NULL AND i.userid = 0 THEN 1
        ELSE 0
    END AS isIssued
,c.categoryid,c.categoryname
			from books as b
			join Category as c
			on c.categoryid=b.categoryid
			left join Users as uu
			on uu.id=b.updatedby
			left join Users as du
			on du.id=b.deletedby
			left join Users as cu
			on cu.id=b.createdby
			LEFT JOIN 
    Issues i ON b.bookid = i.bookid AND i.userid = 0
AND 
    i.status = 'Issued'
			where (@bookid1=0 or b.bookid=@bookid1) and b.isdeleted=0
			order by b.createdon desc

-- trying to issue a books  using sp
declare @errormsg nvarchar(max),@issuccess3 bit
exec spIssueBook 10,2,@errormsg out,@issuccess3 out
select @errormsg as msg,@issuccess3 as success

select DATEDIFF(day,'2024-10-20',GETDATE())

-- trying to return the book using sp

declare @errormsg2 nvarchar(max),@issuccess4 bit
exec spReturnBook 4,27,@errormsg2 output,@issuccess4 output
select @errormsg2 as msg,@issuccess4 as success
select * from Issues

update Issues set issuedate='2024-09-10',duedate='2024-09-15' where userid=7 and bookid=7

-- select query for pagination
declare @bookid2 int =0
declare @sort int=5
select b.*,case 
				when i.bookid is not null and i.userid = 27 then 1
				else 0
			end as isIssued
,c.categoryid,c.categoryname
from books as b
join Category as c
on c.categoryid=b.categoryid
left join Issues as i
on i.bookid=b.bookid and i.userid=27 and i.status='Issued'
where (@bookid2=0 or b.bookid=@bookid2) and (b.isdeleted=0 or (b.isdeleted=1 and i.userid=27)) 
and b.title like '%'+null+'%' and b.authorname like '%%' and c.categoryname like '%%'
order by 
	case @sort
			-- latest books
			when 0 then b.createdon 
	end desc,
	case @sort 
			-- by a -z
			when 1 then b.title
	end asc,
	case @sort
		-- by z-a
		when 2 then b.title
	end desc,
	case @sort
		-- newest added
		when 3 then b.createdon
	end desc,
	case @sort
		-- oldest added
		when 4 then b.createdon
	end asc,
	case @sort	
		-- publication oldest
		when 5 then b.publicationYear
	end asc,
	case @sort
		-- publication newest
		when 6 then b.publicationYear
	end desc




-- for pagination
offset 24 rows
fetch next 12 rows only

-- pagination using sp
exec spGetBooks 0,3,0,0,12,0,@category=0

-- getting all the issued books by user
exec spGetIssuedBooksbyuser 3

-- select query for all the issues
declare @status nvarchar(50)='Issued';
declare @title nvarchar(50)=null;
declare @name nvarchar(50)=null;

select count(id) from Issues where status='Issued'
select sum(fineamount) from Issues where status='LateReturn'


with issuedetails as
(
	select i.*,b.title,CONCAT_WS(' ',u.firstname,u.lastname) as IssuedName
	from Issues as i
	join Books as b
	on i.bookid=b.bookid
	join Users as u
	on u.id=i.userid
)
select i.id,i.userid,i.bookid,i.issuedate,i.duedate,case when status='Issued' and DATEDIFF(day,duedate,GETDATE())>0  then 'DuePassed'
			else status
		end as status,
i.returneddate,i.fineamount,i.dailyfineamount,i.title,i.IssuedName,
count(i.id) over() as TotalRecords
from issuedetails as i
where 
(@status is null or i.status=@status) 
and (@title is null or i.title like '%'+@title+'%')
and (@name is null or i.IssuedName like '%'+@name+'%')
order by i.issuedate desc
offset 10 rows
fetch next 10 rows only

-- same using stored procedure
declare @totalfine int
declare @totalissued int
execute spDashboard null,null,null,0,0,@totalfine out,@totalissued out
select @totalfine fine,@totalissued issued

declare @issucccc bit,@errormsg3 nvarchar(max)
exec spReturnBook 9,17,@errormsg3 out,@issucccc out
select @issucccc as success, @errormsg3 as msg

select * from Issues where userid=9 and bookid=16 and status='Issued'

-- listing all the stored procedures
select * from sys.procedures

update Issues set duedate='2024-05-12',issuedate='2024-03-10' where userid=2 and bookid=3

-- trying to delete the book
declare @issucess45 int
exec spDeleteBook 11,1,@issucess45 output
select @issucess45

-- getting all the issued books of user
declare @status12 nvarchar(40)='DuePassed';
with userissuedBooks
as
(
	select i.id,i.userid,i.issuedate,i.duedate
	,case 
		when status='Issued' and DATEDIFF(day,duedate,GETDATE())>0  then 'DuePassed'
	else status
		end as status,i.returneddate,i.fineamount,i.dailyfineamount,
		b.bookid,b.title,b.isbn,b.authorname,b.coverimage,b.publicationYear,b.availableQty,b.originalQty,b.createdon,b.createdby,b.updatedon,b.updatedby,b.isdeleted,b.deletedon,b.deletedby,c.categoryid,c.categoryname
	from Issues as i
	join Books as b
	on i.bookid=b.bookid
	join Category as c
	on c.categoryid=b.categoryid
	where i.userid=7
)
select * from userissuedBooks
where (@status12 is null or status=@status12)
order by issuedate desc

-- same using sp
exec spGetIssuedBooksByUser 7

-- select query to return all books
select * from Books where isdeleted=1

declare @issucced bit,@insertedid int 
exec spCreateOrGetCategory 'newtry3',@issucced output,@insertedid output
select @issucced as success,@insertedid idinserted


delete from Category where categoryid in(
select categoryid from Category
where categoryid between 10 and 35
)