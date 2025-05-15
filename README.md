# Library Management System

![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![CSS3](https://img.shields.io/badge/css3-%231572B6.svg?style=for-the-badge&logo=css3&logoColor=white)
![HTML5](https://img.shields.io/badge/html5-%23E34F26.svg?style=for-the-badge&logo=html5&logoColor=white)
![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E)
![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![Bootstrap](https://img.shields.io/badge/bootstrap-%238511FA.svg?style=for-the-badge&logo=bootstrap&logoColor=white)
![jQuery](https://img.shields.io/badge/jquery-%230769AD.svg?style=for-the-badge&logo=jquery&logoColor=white)

A comprehensive web-based application developed using .NET Framework 4.8 and MVC5 with a SQL Server backend. The frontend is designed with Bootstrap 3.4.1 for responsive UI and jQuery for dynamic interactions.

### Features:

1. Admin:
   - Manage users and books.
   - Configure library settings, including:
     - Maximum books a user can issue.
     - Fine amount for overdue books.
     - Loan duration for issued books.
2. Librarian:
   - Manage book inventory (add, update, delete books).
   - Access a dashboard to monitor issued and returned books.
3. User:
   - Issue and return books.
   - Pay fines for overdue returns to regain borrowing privileges.

### Screenshots:

1. Login Page
   ![Login](./Images/ss1.png)

2. Browse Book Page
   ![Browse Book](./Images/ss2.png)

3. Browse Book on Admin Page
   ![Browse Book Admin Page](./Images/ss3.png)

4. Book Details
   ![Book Details](./Images/ss4.png)

5. Books Issued by User
   ![Books Issued by User](./Images/ss5.png)

### Output Video : [Click Here](https://www.youtube.com)

### System Tables Architecture Diagram:

![Database Diagram](./Images/dbdiagram.png)

### Installation / Setup:

1. Creating the Database or Running the Scripts:
    - Make sure you have installed the Microsoft SQL Server Database. 
    - Just run the Script Present in file `SQLScripts/Creation_Scripts.sql`

2. Open the Project which is `MVC_Task` on Microsoft Visual Studio or any IDE. 

    1. Make some Changes in the `Web.config` file

    ```
    <add name="database" connectionString="Server=localhost;Database=databasename;Trusted_Connection=true;TrustServerCertificate=True;Data Source=datasource_name"/>

    ```

    2. Don't Change Anything Else. Otherwise there may be some chances of getting the error while running the project.

    3. Before it Install all the required dependencies from the `Nuget Package Manager`.

3. Run the Project 
4. If you want to login as the Admin Enter the below Credentials
    ```
    Username: admin
    Passsword: admin@123
    ```



## Author: Prathamesh Dhande
