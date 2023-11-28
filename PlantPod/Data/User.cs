using Microsoft.Extensions.Configuration;
using PlantPod.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantPod.Data;

namespace PlantPod.Data
{
    public class User
    {
        private int id = 0;
        private string username;
        private DataLibrary.IDataAccess _data;
        public User(DataLibrary.IDataAccess dataAccess) => _data = dataAccess;
        public bool CheckLogin()
        {
            // Check if id == 0 if so user is not logged in
            if (id == 0)
            {
                return false;
            }
            return true;
        }

        public int getId()
        {
            return id;
        }

        public string getUsername()
        {
            return username;
        }

        public async Task<List<string>> LoginUser(string inputUsername, string inputPassword)
        {
            List<string> Messages = new List<string>();
            string hash = null;

            // Check if inputs are filled in
            if (inputUsername == null || inputUsername == "")
            {
                Messages.Add("Vul een gebruikersnaam in");
            }
            if (inputPassword == null || inputPassword == "")
            {
                Messages.Add("Vul een wachtwoord in");
            }

            // If there are no error messages, continue
            if (Messages.Count == 0)
            {
                // Get user with given username
                List<UserModel> users = await _data.LoadData<UserModel, dynamic>("select password, id from users where username=@username", new { username = inputUsername }, ConnectionData.ConnectionString);

                // If user was found
                if (users != null)
                {
                    foreach (var u in users)
                    {
                        // Save data from found user
                        hash = u.Password;
                        id = u.Id;
                    }

                    // Password not existing means the user was not found
                    if (hash == null || hash == "")
                    {
                        Messages.Add("Gebruiker bestaat niet");
                    }
                    else
                    {
                        // Verify the given password with the one retrieved from given username
                        if (BCrypt.Net.BCrypt.Verify(inputPassword, hash))
                        {
                            // Save username
                            username = inputUsername;
                        }
                        else
                        {
                            Messages.Add("Incorrect wachtwoord");
                        }
                    }
                }
            }
            return Messages;
        }

        public void logoutUser()
        {
            // Make id = 0 which means there is no user + remove username
            id = 0;
            username = null;
        }
        public async Task<List<string>> registerUser(string inputUsername, string inputPassword, string inputPasswordConfirmation)
        {
            List<string> Messages = new List<string>();
            string hash = null;
            username = null;

            // Check if inputs are filled in
            if (inputUsername == null || inputUsername == "")
            {
                Messages.Add("Vul een gebruikersnaam in");
            }
            if (inputPassword == null || inputPassword == "")
            {
                Messages.Add("Vul een wachtwoord in");
            }
            if (inputPasswordConfirmation == null || inputPasswordConfirmation == "")
            {
                Messages.Add("Vul de wachtwoord verificatie in");
            }

            // If there are no error messages, continue to check if data meets requirements
            if (Messages.Count == 0)
            {
                if (inputUsername.Length < 3)
                {
                    Messages.Add("Gebruikersnaam moet gelijk of langer zijn dan 3 karakters");
                }
                if (inputPassword.Length < 5)
                {
                    Messages.Add("Wachtwoord moet gelijk of langer zijn dan 5 karakters");
                }
                if (inputPassword != inputPasswordConfirmation)
                {
                    Messages.Add("De twee wachtwoorden komen niet overeen");
                }
            }

            // If there are no error messages, continue
            if (Messages.Count == 0)
            {
                // Get user with given username to check username availability
                List<UserModel> users = await _data.LoadData<UserModel, dynamic>("select username from users where username=@username", new { username = inputUsername }, ConnectionData.ConnectionString);

                // If no users were found with the same username, continue
                if (users != null)
                {
                    foreach (var u in users)
                    {
                        // Save username
                        username = u.Username;
                    }
                    // Username existing means the username isn't available
                    if (username != null && username != "")
                    {
                        Messages.Add("Gebruikersnaam is al in gebruik");
                    }
                    else
                    {
                        // Hash the password and insert the data into the database
                        hash = BCrypt.Net.BCrypt.HashPassword(inputPassword);
                        await _data.SaveData("insert into users (username, password) values (@username, @password);", new { username = inputUsername, password = hash }, ConnectionData.ConnectionString);
                    }
                }
            }
            return Messages;
        }
    }
}
