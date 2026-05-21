### **a) GitHub Repository:** [omarzareeef/ProjectManagementSystem](https://github.com/omarzareeef/ProjectManagementSystem/tree/main)



\----------------------------------------------------------------------------------------------------------------------------



### **b) Setup Instructions** 



**\*\*you need to open docker desktop\*\*.**

1\. open command prompt(CMD) in the root folder of the project and run "docker compose up". or click docker compose directly from VS.

&#x20;  => it will take some seconds to start containers for (MS SQL Server, PMS.API). you will find swagger on: [https://localhost:8081/swagger/index.html](https://localhost:8081/swagger/index.html)



**\*\*hint\*\***

##### &#x20; **Skip the coming steps(2, 3) by running this command on MS SQL Server**

###### &#x20; **Connect to this server:**

&#x09;Server name: localhost,1433

&#x09;Authentication: SQL Server Authentication

&#x09;Login: sa

&#x09;Password: Strong@Passw0rd



###### &#x20; **Run this command:**

&#x09;USE \[PmsDb]

&#x09;GO



&#x09;INSERT INTO \[dbo].\[AspNetUsers]

&#x20;          (\[Id]

&#x20;          ,\[UserName]

&#x20;          ,\[NormalizedUserName]

&#x20;          ,\[Email]

&#x20;          ,\[NormalizedEmail]

&#x20;          ,\[EmailConfirmed]

&#x20;          ,\[PasswordHash]

&#x20;          ,\[SecurityStamp]

&#x20;          ,\[ConcurrencyStamp]

&#x20;          ,\[PhoneNumber]

&#x20;          ,\[PhoneNumberConfirmed]

&#x20;          ,\[TwoFactorEnabled]

&#x20;          ,\[LockoutEnd]

&#x20;          ,\[LockoutEnabled]

&#x20;          ,\[AccessFailedCount])

&#x20;    	VALUES

&#x20;          (NEWID()

&#x20;          ,'*your email*'

&#x20;          ,'*your email Capitalized*'

&#x20;          ,'*your email*'

&#x20;          ,'*your email Capitalized*'

&#x20;          ,1

&#x20;          ,'AQAAAAIAAYagAAAAELUMeYOu3o5Y08vdYhcda5CPWuNAEHe1oHBZsAJu4sPvtCnszJXFzL2zNFZAY4IX/A=='  --Original Value for login => Qq11111!

&#x20;          ,'ELYJZ54MEDKZBJJWFBNDA7VEZ236IO2T'

&#x20;          ,'02b32998-5368-42ba-8f84-2e54b4d98b32'

&#x20;          ,NULL

&#x20;          ,0

&#x20;          ,0

&#x20;          ,NULL

&#x20;          ,1

&#x20;          ,0)

&#x09;GO

&#x20; **If you did this skip to step 4 :)**



2\. register with your email.

&#x20;  => you will get a confirmation email.



3\. confirm your email by clicking on the link in the confirmation email.



4\. login with your credentials.

&#x20;  => now you got full access on the endpoints.



\----------------------------------------------------------------------------------------------------------------------------



### **c) Architecture Overview** 



1. I build a clean architecture for its benefits(Maintainability, testability, Separation Of Concern). It is a backend project built with ASP.NET Core 10 Web API with Onion architecture (Domain / Application / Infrastructure / API), EF Core + SQL Server, repository + unit-of-work.



2\. I used docker compose to start MS SQL Server and API in containers.



3\. I used MS Identity server for handling User Management.



4\. I added refresh token functionality so the client will use it every time the access token expire without the need for relogging.



5\. I used cookies as its more secured so the client don't need to handle anything regarding the tokens. for getting user info, make use of User/GetInfo() endpoint.



6\. for test project in the future, it will be added in the tests folder for the sake of file structure organizing.

