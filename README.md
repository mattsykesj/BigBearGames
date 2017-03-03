# BigBearGames

My first time implementing Microsoft Identity through the OWIN framework, this allows administrators CRUD operations on both users and roles. There are 3 roles, Admins, Bloggers, Users. Bloggers can reach a blog dash board on the navbar after logging in where they can perform CRUD operations on their own blogs which show on the main page. Users can perform CRUD operations on comments to articles, reset there own email and/or password from there user dashboard. Users must sign up before posting comments, the signup page sends an email to the users to thank them for registering, the form validation is a combination of the password and user validator provided by Identity and data annotations for the view models. The article pages all have pagination showing two articles per page at the moment but can easily be changed by one parameter to allow for scalability. Custom maproute applied to those pages to remove query strings as to be more user friendly. The SQL database is accessed by Entity Framework and LINQ, basic formatting through bootstrap. Still an ongoing project but a good foundation in place.

Logins for testing/viewing

User                      Blogger                         Administrator
UserName: User1           UserName: Blogger               UserName: Admin
Password: Secret1         Password: Secret1               Password: Secret1          
