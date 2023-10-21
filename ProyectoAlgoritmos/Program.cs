using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Transactions;

namespace Cajero
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string userfilepath = @"C:\Proyectos Visual\ProyectoAlgoritmos\Main\Usuarios\Users.txt";
            string adminfilepath = @"C:\Proyectos Visual\ProyectoAlgoritmos\Main\Usuarios\Admin.txt";
            string historyFilePath = @"C:\Proyectos Visual\ProyectoAlgoritmos\Main\Usuarios\History.txt";
            
            List<Historial> TransactionHistory = CargarTransacciones(historyFilePath);

            while (true)
            {
                string username, password = string.Empty;

                while (true)
                {
                    Console.WriteLine("__________________");
                    Console.WriteLine("|Login del Cajero|");
                    Console.WriteLine("------------------");

                    Console.Write("Digite su número de tarjeta o Ingrese su Nombre de Usuario: ");
                    username = Console.ReadLine();

                    Console.Write("Ingrese su contraseña: ");
                    password = Console.ReadLine();
                    Console.Clear();

                    if (Admin(username, password, adminfilepath))
                    {
                        int n;
                            
                        while (true)
                        {
                            Console.WriteLine("_____________________");
                            Console.WriteLine("|Simulador de Cajero|");
                            Console.WriteLine("---------------------");
                            Console.WriteLine("");
                            Console.WriteLine("¡Bienvenido, Administrador!");
                            Console.WriteLine("Escoje la Opción que necesites: ");
                            Console.WriteLine("1. Inicar Sesión");
                            Console.WriteLine("2. Crear una Cuenta Nueva");
                            Console.WriteLine("3. Cerrar Sesión");
                        if(int.TryParse(Console.ReadLine(), out n))
                            {
                                if (n >= 1 && n <= 3)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Escriba una opción válida (1, 2 o 3).");
                                }
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Escriba una opción válida (1, 2 o 3).");
                            }
                        }
                        if (n == 1)
                        {
                            if (Login(username, password, adminfilepath))
                            {
                                Console.Clear();
                                Console.WriteLine("Inicio de Sesión Exitoso como Administrador.");
                                break;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Nombre de Administrador o Contraseña Incorrecta.");
                                
                            }
                        }
                        else if (n == 2)
                        {
                            Console.WriteLine("Crear Nueva Cuenta de Usuario");
                            Console.Write("Ingrese el número de tarjeta que va a asignar: ");
                            string newusername = Console.ReadLine();
                            Console.Write("Ingrese su Contraseña: ");
                            string newpassword = Console.ReadLine();

                            if (CreateUser(newusername, newpassword, userfilepath))
                            {
                                Console.Clear();
                                Console.WriteLine("Cuenta creada con éxtio.");
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("No se logró crear el nuevo usuario.");
                            }
                        }
                        else if (n == 3)
                        {
                            Console.Clear();
                            Console.WriteLine("Sesion Cerrada");

                        }
                    }
                    else
                    {
                        if (Login(username, password, userfilepath))
                        {
                            Console.Clear();
                            Console.WriteLine("Incio de Sesión Exitoso.");
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Nombre de Usuario, Número de Tarjeta o Contraseña Incorrecta.");
                          
                        }
                    }
                }
            
               
                bool loggedIn = true;

                while (loggedIn)
                {
                    int choice;
                    while (true)
                    {
                        Console.WriteLine("_____________________");
                        Console.WriteLine("|Simulador de Cajero|");
                        Console.WriteLine("---------------------");
                        Console.WriteLine("");
                        Console.WriteLine($"¡Bienvenido, {username}!");
                        Console.WriteLine("Eliga la Accion que desea Realizar.");
                        Console.WriteLine("1. Consulta de Saldo");
                        Console.WriteLine("2. Retirar Dinero");
                        Console.WriteLine("3. Depositar Dinero");
                        Console.WriteLine("4. Ver Historial de Transacciones");
                        Console.WriteLine("5. Cerrar Sesion");

                        if (int.TryParse(Console.ReadLine(), out choice))
                        {
                            if (choice >= 1 && choice <= 5)
                            {
                                break;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Escriba una opción válida (1, 2, 3, 4 o 5).");
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Escriba una opción válida (1, 2, 3, 4 o 5).");
                        }
                    }

                    switch (choice)
                    {
                        case 1:
                            Console.Clear();
                            Console.WriteLine("Consulta de Saldo");
                            decimal saldo = ConsultaSaldo(username, userfilepath, adminfilepath);
                            Console.WriteLine($"Saldo actual: Q.{saldo}");

                            break;

                        case 2:
                            Console.Clear();
                            Console.WriteLine("Retirar Dinero");
                            Console.Write("Ingrese la cantidad a retirar: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                            {
                                Console.Write($"¿Desea realizar el retiro de Q.{withdrawAmount}? (Si/No): ");
                                string confirmation = Console.ReadLine().Trim().ToLower();

                                if (confirmation == "si")
                                {
                                    if (withdrawAmount > 0)
                                    {
                                        if (RetirarDinero(username, withdrawAmount, userfilepath, adminfilepath, TransactionHistory, historyFilePath))
                                        {
                                            Console.WriteLine($"Se han retirado Q.{withdrawAmount}");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Fondos insuficientes para realizar el retiro.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Cantidad ingresada no válida.");
                                    }
                                }
                                else if (confirmation == "no")
                                {
                                    Console.WriteLine("Operación de retiro cancelada.");
                                }
                                else
                                {
                                    Console.WriteLine("Respuesta no válida. Operación de retiro cancelada.");
                                }
                            }
                            break;


                            case 3:
                            Console.Clear();
                            Console.WriteLine("Depositar Dinero");
                            Console.Write("Ingrese la cantidad a depositar: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                            {
                                Console.Write($"¿Desea realizar el depósito de Q.{depositAmount}? (Si/No): ");
                                string confirmation = Console.ReadLine().Trim().ToLower();

                                if (confirmation == "si")
                                {
                                    if (depositAmount > 0)
                                    {
                                        DepositarDinero(username, depositAmount, userfilepath, adminfilepath, TransactionHistory, historyFilePath);
                                        Console.WriteLine($"Se han depositado Q.{depositAmount}");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Cantidad ingresada no válida.");
                                    }
                                }
                                else if (confirmation == "no")
                                {
                                    Console.WriteLine("Operación de depósito cancelada.");
                                }
                                else
                                {
                                    Console.WriteLine("Respuesta no válida. Operación de depósito cancelada.");
                                }
                            }
                            break;



                        case 4:
                            Console.Clear();
                            
                            MostrarTransacciones(historyFilePath, username);
                            break;

                        case 5:
                            Console.Clear();
                            Console.WriteLine("Sesion Cerrada");
                            loggedIn = false;
                            break;

                        default:
                            Console.WriteLine("Opción no válida");
                            break;
                    }
                    
                }

            }


        }


            static bool Admin(string username, string password, string adminFilePath)
            {
                if (File.Exists(adminFilePath))
                {
                    string[] adminInfo = File.ReadAllText(adminFilePath).Split('|');
                    if (adminInfo.Length == 3 && adminInfo[0] == username && adminInfo[1] == password)
                    {
                        return true;
                    }
                }
                return false;
            }

            static bool Login(string username, string password, string usersFilePath)
            {
                if (File.Exists(usersFilePath))
                {
                    string[] lines = File.ReadAllLines(usersFilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3 && parts[0] == username && parts[1] == password)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            static bool CreateUser(string username, string password, string usersFilePath)
            {
                if (File.Exists(usersFilePath))
                {
                    File.AppendAllText(usersFilePath, username + "|" + password + "|0.00" +Environment.NewLine);
                    return true;
                }
                return false;
            }

            static decimal ConsultaSaldo(string username, string usersFilePath, string adminFilePath)
            {

                if (File.Exists(usersFilePath))
                {
                    string[] lines = File.ReadAllLines(usersFilePath);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split('|');
                        if (parts.Length == 3 && parts[0] == username)
                        {
                            if (decimal.TryParse(parts[2], out decimal saldo))
                            {
                                return saldo;
                            }
                        }
                    }
                }
                if (File.Exists(adminFilePath))
                {
                        string[] lines = File.ReadAllLines(adminFilePath);
                        foreach (string line in lines)
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length == 3 && parts[0] == username)
                            {
                                if (decimal.TryParse(parts[2], out decimal saldo))
                                {
                                    return saldo;
                                }
                            }
                        }
                    }
                return 0.0m;
            }

        static void DepositarDinero(string username, decimal amount, string usersFilePath, string adminFilePath, List<Historial> transactionHistory, string userHistoryFilePath)
        {
            if (File.Exists(usersFilePath))
            {
                string[] lines = File.ReadAllLines(usersFilePath);
                List<string> updatedLines = new List<string>();

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 3 && parts[0] == username)
                    {
                        if (decimal.TryParse(parts[2], out decimal saldo))
                        {
                            RegistrarTransaccion(username, "Deposito", amount, transactionHistory, userHistoryFilePath);
                            saldo += amount;
                            parts[2] = saldo.ToString();
                        }
                    }
                    updatedLines.Add(string.Join("|", parts));
                }

                File.WriteAllLines(usersFilePath, updatedLines);
            }
            if (File.Exists(adminFilePath))
            {
                string[] lines = File.ReadAllLines(adminFilePath);
                List<string> updatedLines = new List<string>();

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 3 && parts[0] == username)
                    {
                        if (decimal.TryParse(parts[2], out decimal saldo))
                        {
                            RegistrarTransaccion(username, "Deposito", amount, transactionHistory, userHistoryFilePath);
                            saldo += amount;
                            parts[2] = saldo.ToString();
                        }
                    }
                    updatedLines.Add(string.Join("|", parts));
                }

                File.WriteAllLines(adminFilePath, updatedLines);
            }
        }

        static bool RetirarDinero(string username, decimal amount, string usersFilePath, string adminFilePath, List<Historial> transactionHistory, string userHistoryFilePath)
        {
            if (File.Exists(usersFilePath))
            {
                string[] lines = File.ReadAllLines(usersFilePath);
                List<string> updatedLines = new List<string>();

                bool fundsSufficient = false;

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 3 && parts[0] == username)
                    {
                        if (decimal.TryParse(parts[2], out decimal saldo))
                        {
                            if (saldo >= amount)
                            {
                                RegistrarTransaccion(username, "Retiro", amount, transactionHistory, userHistoryFilePath);

                                saldo -= amount;
                                parts[2] = saldo.ToString();
                                fundsSufficient = true;
                            }
                        }
                    }
                    updatedLines.Add(string.Join("|", parts));
                }

                if (fundsSufficient)
                {
                    File.WriteAllLines(usersFilePath, updatedLines);
                    return true;
                }
            }
            if (File.Exists(adminFilePath))
            {
                string[] lines = File.ReadAllLines(adminFilePath);
                List<string> updatedLines = new List<string>();

                bool fundsSufficient = false;

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 3 && parts[0] == username)
                    {
                        if (decimal.TryParse(parts[2], out decimal saldo))
                        {
                            if (saldo >= amount)
                            {
                                RegistrarTransaccion(username, "Retiro", amount, transactionHistory, userHistoryFilePath);

                                saldo -= amount;
                                parts[2] = saldo.ToString();
                                fundsSufficient = true;
                            }
                        }
                    }
                    updatedLines.Add(string.Join("|", parts));
                }

                if (fundsSufficient)
                {
                    File.WriteAllLines(adminFilePath, updatedLines);
                    return true;
                }
            }

            return false;
        }

        struct Historial
        {
            public string Username;
            public DateTime Date;
            public string Type;
            public decimal Amount;
        }
        static void RegistrarTransaccion(string username, string type, decimal amount, List<Historial> transactionHistory, string historyFilePath)
        {
            Historial transaction = new Historial
            {
                Username = username,
                Date = DateTime.Now,
                Type = type,
                Amount = amount
            };
            transactionHistory.Add(transaction);

            File.AppendAllText(historyFilePath, $"{transaction.Username}|{transaction.Date}|{transaction.Type}|{transaction.Amount}" + Environment.NewLine);
        }
        static List<Historial> CargarTransacciones(string historyFilePath)
        {
            List<Historial> transactions = new List<Historial>();
            if (File.Exists(historyFilePath))
            {
                string[] lines = File.ReadAllLines(historyFilePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        Historial transaction = new Historial
                        {
                            Date = DateTime.Parse(parts[1]),
                            Type = parts[2],
                            Amount = decimal.Parse(parts[3])
                        };
                        transactions.Add(transaction);
                    }
                }
            }
            return transactions;
        }

        static void MostrarTransacciones(string historyFilePath, string username)
        {
            Console.WriteLine("Historial de Transacciones:");

            if (File.Exists(historyFilePath))
            {
                string[] lines = File.ReadAllLines(historyFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 4 && parts[0] == username)
                    {
                        DateTime date = DateTime.Parse(parts[1]);
                        string transactionType = parts[2];
                        decimal amount = decimal.Parse(parts[3]);
                        Console.WriteLine($"Fecha: {date}, Tipo: {transactionType}, Monto: Q.{amount}");
                    }
                }
            }
        }

    }

}