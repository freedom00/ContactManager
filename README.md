# ContactManager
Initial Project

If you haven't already set a password for seeded user accounts, use the Secret Manager tool to set a password:

      (a). Choose a strong password: Use eight or more characters and at least one upper-case character, number, and symbol. For example, Passw0rd! meets the strong password requirements.


      (b). Execute the following command from the project's folder, where <PW> is the password:

           dotnet user-secrets init
           dotnet user-secrets set SeedUserPW <PW>
