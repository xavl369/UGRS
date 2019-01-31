using System;
using System.Data.Entity;
using UGRS.Core.Auctions.Entities.Users;
using UGRS.Core.Extension.Security;
using UGRS.Data.Auctions.Context;
using System.Linq;
using UGRS.Core.Utility;
using UGRS.Core.Auctions.DTO.Session;

namespace UGRS
{
    public class EntityFrameworkTransactionTest
    {
        #region Attributes

        //AuctionsContext mObjAuctionsContext;

        #endregion

        public EntityFrameworkTransactionTest()
        {
            StaticSessionUtility.mObjSeccion = new SessionDTO()
            {
                Id = 0,
                UserName = "AuctionsService"
            };
            //Console.WriteLine("Creando contexto....");
            //mObjAuctionsContext = new AuctionsContext();
            //Console.WriteLine("Contexto creado");
        }

        public void DoTest()
        {
            using (var lObjContext = new AuctionsContext())
            {
                Console.WriteLine("Creado transacción....");

                var a = lObjContext.User.ToList();

                using (var lObjTransaction = lObjContext.Database.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("Transacción creada");
                        EETransactionDAO<User> lObjUserDAO = new EETransactionDAO<User>(lObjContext);

                        for (int i = 0; i < 5; i++)
                        {
                            lObjUserDAO.SaveOrUpdateEntity(new User()
                            {
                                UserTypeId = 1,
                                UserName = string.Format("User {0}", i),
                                FirstName = string.Format("Nombre {0}", i),
                                LastName = string.Format("Apellido {0}", i),
                                Password = string.Format("Contraseña {0}", i).Encode(),
                                EmailAddress = string.Format("Correo{0}@email.com", i).Encode(),
                                Image = string.Empty
                            });

                            Console.WriteLine(string.Format("Usuario {0} creado", i));
                        }

                        lObjTransaction.Commit();

                        Console.WriteLine("Cambios aplicados");
                    }
                    catch (Exception lObjException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(lObjException.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
            }
        }
    }
}
