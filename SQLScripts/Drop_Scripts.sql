/** Drop Scripts **/

/* Droping Stored Procedure */

-- drop the Stored Procedure of Register User
drop proc spRegisterUser


/* Droping Views */

-- drop the view of login details
drop view vw_LoginDetails

/* Droping Tables */

-- drop issues table
drop table issues

-- drop rolemapping table
drop table RoleMapping

alter table RoleMapping
drop constraint FK_RoleMapping_userid

-- drop users table
drop table Users

-- dropping the database
drop database LibraryDB

-- drop the books issued by user stored procedure
drop proc spGetIssuedBooksByUser