using ApiProf_A.Models;
using System.Runtime.CompilerServices;
using System;
using System.DirectoryServices;

namespace ApiProf_A.Services
{
    public class ActiveDirectoryService
    {
        private readonly string _ldapPath;
        private readonly string _username;
        private readonly string _password;

        public ActiveDirectoryService(string ldapPath, string username, string password="Tecc#1234")
        {
            //le mot de passe defaut Tecc#1234 pour initialiser la variable seulement, et lorsque on ajoutera le professeur, la fonction generer...
            _ldapPath = ldapPath;
            _username = username;
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public void AddProfessor(Professeur professeur) {
            //PAS FINI, LES CHAMPS SONT TOUS NULS
            using (var entry = new DirectoryEntry(_ldapPath, _username, _password))
            {
                using (var newUser = entry.Children.Add($"CN={professeur.prenomProf} {professeur.nomProf}", "user"))//configure le DN
                {
                    newUser.Properties["givenName"].Value = professeur.prenomProf;
                    newUser.Properties["sn"].Value = professeur.nomProf;
                    newUser.Properties["sAMAccountName"].Value = professeur.idProf;
                    newUser.Properties["userPrincipalName"].Value = $"{professeur.idProf}@teccart.com";
                    newUser.Properties["department"].Value = professeur.departement;
                    newUser.CommitChanges();

                    string mdp = ApiProf_A.Services.GenerateurMotDePasse.generationMotDePasse();
                    newUser.Invoke("SetPassword", new object[] { mdp });
                    newUser.Properties["userAccountControl"].Value = 0x200; // Enable account
                    newUser.CommitChanges();
                }
            }
        }

    }
}
