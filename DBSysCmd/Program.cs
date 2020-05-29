using System;
using Microsoft.Extensions.CommandLineUtils;

using DBSysCore;

namespace DBSysCmd
{
    /**
     * Главный статический класс программы.
     */
    public static class Program
    {
        /**
         * Точка входа программы.
         * 
         * Парсит переданные аргументы и выполняет найденные команды.
         */
        private static int Main(string[] args)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.Main");

            CommandLineApplication app = new CommandLineApplication
            {
                Name = "DBSysCore",
                Description = "desc."
            };
            app.HelpOption("-?|-h|--help");

            app.Command("status", StatusCommand);

            // comands for auth

            app.Command("login", LoginCommand);

            app.Command("logout", LogoutCommand);

            // testers commands

            app.Command("challenge", ChallengeCommand);

            app.Command("test", TestCommand);

            // operator commands

            app.Command("dump-use", DumpUseCommand);

            app.Command("dump-merge", DumpMergeCommand);

            app.Command("update-static", UpdateStaticCommand);

            // admin commands

            app.Command("staff-add", StaffAddCommand);

            app.Command("sql", SqlCommand);

            app.OnExecute(() =>
            {
                app.ShowHint();
                return 0;
            });

            return app.Execute(args);
        }

        /**
         * Команда проверки текущего состояния программы.
         * 
         * Аргументов и опций не требуется.
         */
        private static void StatusCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.StatusCommand");

            command.Description = "Shows current state of the program.";
            command.HelpOption("-?|-h|--h");

            command.OnExecute(() =>
            {
                if (Core.Status(out string status) == StatusCode.Ok)
                    Console.WriteLine(status);
                else
                    Console.WriteLine("Error happened!");

                return 0;
            });
        }

        /**
         * Команда для аутентификации текущего пользователя.
         */
        private static void LoginCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.LoginCommand");

            command.Description = "Login in to gain access to all other commands of this app.";
            command.HelpOption("-?|-h|--h");

            CommandArgument login = command.Argument("<login>", "user login");
            CommandArgument pass = command.Argument("<password>", "user password");

            command.OnExecute(() =>
            {
                if (login.Value == null || pass.Value == null)
                    command.ShowHelp();
                else
                {
                    switch (Core.Login(login.Value, pass.Value))
                    {
                        case StatusCode.Ok:
                            Console.WriteLine("Successfully authorized!");
                            break;

                        case StatusCode.LoginAlreadyAuthorized:
                            Console.WriteLine("You can't login again. First logout.");
                            break;

                        case StatusCode.LoginInvalidLogin:
                            Console.WriteLine("Invalid login. Try again.");
                            break;

                        case StatusCode.LoginInvalidPass:
                            Console.WriteLine("Invalid password. Try again.");
                            break;

                        case StatusCode.LoginNoRegisteredUsers:
                            Console.WriteLine("No registered users!");
                            Console.WriteLine("Add users using virtual admin.");
                            break;

                        default:
                            Console.WriteLine("Unexpected error!");
                            break;
                    }
                }

                return 0;
            });
        }

        /**
         * Комадна для сброса текущей сессии.
         */
        private static void LogoutCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.LogoutCommand");

            command.Description = "Command for logging out.";
            command.HelpOption("-?|-h|--h");

            command.OnExecute(() =>
            {
                Core.Logout();
                return 0;
            });
        }

        /**
         * Команда для работы с активной последовательностью тестов.
         * 
         * Сигнатура комманды:
         *      
         *      challenge -b|--begin -p|--product="" -c|--challenge-type="" -l|--location="" -d|--description=""
         *      challenge -e|--end
         */
        private static void ChallengeCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.ChallengeCommand");

            command.Description = "Start processing tests sequence.";
            command.HelpOption("-?|-h|--help");

            // add arguments indicating beginning and ending of testing
            CommandOption beginOpt = command.Option("-b|--begin",
                "Used for begining test sequence.", CommandOptionType.NoValue);
            CommandOption endOpt = command.Option("-e|--end",
                "Used for ending test sequence.", CommandOptionType.NoValue);

            CommandOption productOpt = command.Option("-p|--product",
                "product name.", CommandOptionType.SingleValue);
            CommandOption challengeTypeOpt = command.Option("-c|--challenge-type",
                "challenge type.", CommandOptionType.SingleValue);
            CommandOption locationOpt = command.Option("-l|--location",
                "location.", CommandOptionType.SingleValue);
            CommandOption descriptionOpt = command.Option("-d|--description",
                "description.", CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                int result = 0;

                if (!beginOpt.HasValue() && !endOpt.HasValue()
                    || beginOpt.HasValue() && endOpt.HasValue())
                {
                    command.ShowHelp();
                }
                else if (beginOpt.HasValue() && !(productOpt.HasValue()
                    && challengeTypeOpt.HasValue() && locationOpt.HasValue()))
                {
                    command.ShowHelp();
                }
                else if (beginOpt.HasValue())
                {
                    // TODO: implement correctlly later
                    switch (Core.BeginChallenge(productOpt.Value(),
                        "coName", "coSerialNumber", "coDecNumber", 0, "coParent",
                        challengeTypeOpt.Value(), locationOpt.Value(),
                        descriptionOpt.HasValue() ? descriptionOpt.Value() : ""))
                    {
                        case StatusCode.Ok:
                            // TODO: add description in console.
                            break;

                        case StatusCode.GrantsInproper:
                            break;

                        case StatusCode.ProgramStateInvalid:
                            break;

                        default:
                        case StatusCode.Error:
                            break;
                    }

                    string logString = $"challenge -b --product=\"{productOpt.Value()}\" " +
                        $"--challenge-type=\"{challengeTypeOpt.Value()}\" --location=\"{locationOpt.Value()}\"" +
                        (descriptionOpt.HasValue() ? $" --description=\"{descriptionOpt.Value()}\"" : "");

                    Logger.Log(logString);
                    Console.WriteLine("Initialized new challenge!");
                }
                else if (endOpt.HasValue())
                {
                    switch (Core.EndChallenge())
                    {
                        case StatusCode.Ok:
                            // TODO: describe
                            Console.WriteLine("Challenge has been added successfully!\n");
                            Logger.Log("challenge --end");
                            break;

                        default:
                        case StatusCode.Error:
                            Console.WriteLine("Error happened while saving current test!");
                            result = -1;
                            break;
                    }
                }

                return result;
            });
        }

        /**
         * Комадна для работы с активным тестом.
         */
        private static void TestCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.TestCommand");

            command.Description = "Process particular test.";
            command.HelpOption("-?|-h|--help");

            // TODO: fix this command

            CommandOption tsIndexOpt = command.Option("-i|--ts-index",
                "TestStand long index string", CommandOptionType.SingleValue);
            CommandOption nominalOpt = command.Option("-n|--nominal",
                "nominal value", CommandOptionType.SingleValue);
            CommandOption actValueOpt = command.Option("-a|--act-value",
                "actual value", CommandOptionType.SingleValue);
            CommandOption deltaOpt = command.Option("-d|--delta",
                "delta", CommandOptionType.SingleValue);
            CommandOption boundaryOpt = command.Option("-b|--bound",
                "boundary value", CommandOptionType.SingleValue);
            CommandOption statusOpt = command.Option("-s|--status",
                "test status 0 or 1", CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                int result = 0;

                bool providedTsIndex = tsIndexOpt.HasValue();
                bool providedValues = nominalOpt.HasValue() && actValueOpt.HasValue() && deltaOpt.HasValue();
                bool providedStatus = statusOpt.HasValue();

                if (!(providedTsIndex && providedStatus))
                    command.ShowHelp();
                else if (!providedValues)
                {
                    switch (Core.Test(tsIndexOpt.Value(), statusOpt.Value() == "1"))
                    {
                        case StatusCode.Ok:
                            // Logger.Log($"test --mode=\"{mode.name}\" --connection-" +
                            //     $"interface=\"{connectionInterface.name}\" --req-n=" +
                            //     $"\"{requirements.name}\" --req-m=\"{methodology.name}" +
                            //     $",{methodology.docNumber}\"");
                            Console.WriteLine("New test added successfully!");
                            break;

                        default:
                        case StatusCode.Error:
                            result = -1;
                            break;
                    }
                }
                else
                {
                    switch (Core.Test(tsIndexOpt.Value(), statusOpt.Value() == "1", Convert.ToDecimal(nominalOpt.Value()),
                        Convert.ToDecimal(actValueOpt.Value()), Convert.ToDecimal(deltaOpt.Value()), Convert.ToDecimal(boundaryOpt.Value())))
                    {
                        // TODO: add description
                        case StatusCode.Ok:
                            // Logger.Log($"test --mode=\"{mode.name}\" --connection-" +
                            //     $"interface=\"{connectionInterface.name}\" --req-n=" +
                            //     $"\"{requirements.name}\" --req-m=\"{methodology.name}" +
                            //     $",{methodology.docNumber}\"");
                            Console.WriteLine("New test added successfully!");
                            break;

                        default:
                        case StatusCode.Error:
                            result = -1;
                            break;
                    }
                }

                return result;
            });
        }

        /**
         * Команда для смены текущего файла дампа.
         */
        private static void DumpUseCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.DumpUseCommand");

            command.Description = "Switches to presented database file (creates if needed).";
            command.HelpOption("-?|-h|--help");

            CommandArgument filename = command.Argument("<filename.db>", "database file to load or init.");

            command.OnExecute(() =>
            {
                int result = 0;

                if (filename.Value == null)
                    command.ShowHelp();
                else
                {
                    switch (Core.DumpUse(filename.Value))
                    {
                        // TODO: add description

                        case StatusCode.Ok:
                            break;

                        default:
                        case StatusCode.Error:
                            result = -1;
                            break;
                    }
                }

                return result;
            });
        }

        /**
         * Коммадна для объединения нескольких файлов дампов в один.
         */
        private static void DumpMergeCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.DumpMergeCommand");

            command.Description = "Merges several dump files into one.";
            command.HelpOption("-?|-h|--help");

            CommandArgument target = command.Argument("<target.db>", "target dump file which will contain all data.");
            CommandArgument source = command.Argument("<source.db>", "source dump to take data from.");

            command.OnExecute(() =>
            {
                int result = 0;

                if (target.Value == null || source.Value == null)
                    command.ShowHelp();
                else
                {
                    switch (Core.DumpMerge(target.Value, source.Value.Split(" ")))
                    {
                        case StatusCode.Ok:
                            Logger.Log($"dump-merge \"{target.Value}\" \"{source.Value}\"");
                            Console.WriteLine("Successfully merged founded files into 1.");
                            break;

                        default:
                        case StatusCode.Error:
                            break;
                    }
                }

                return result;
            });
        }

        /**
         * Команда для загрузки статических данных в текущий файл дампа.
         */
        private static void UpdateStaticCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.UpdateStaticCommand");

            command.Description = "Updates all static data in database from static.xls file.";
            command.HelpOption("-?|-h|--help");
            command.OnExecute(() =>
            {
                int result = 0;

                StatusCode status = Core.LoadStaticTests();
                switch (status)
                {
                    case StatusCode.Ok:
                        Logger.Log("update-static");
                        break;

                    default:
                    case StatusCode.Error:
                        // TODO: describe
                        Console.WriteLine($"Unexpected error happened with code = {status}");
                        result = -1;
                        break;
                }

                return result;
            });
        }

        /**
         * Команда для добавления данных пользователей.
         */
        private static void StaffAddCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.StaffAddCommand");

            command.Description = "Adds new staff into database. Returns personID.";
            command.HelpOption("-?|-h|--help");
            CommandArgument sname = command.Argument("<surname>", "staff surname");
            CommandArgument fname = command.Argument("<first-name>", "staff first name");
            CommandArgument pname = command.Argument("<patronymic-name>", "staff patronymic name");
            CommandArgument post = command.Argument("<post>", "string representing staff post (tester, operator, admin)");
            CommandArgument depr = command.Argument("<department>", "string representing staff department");
            CommandArgument login = command.Argument("<login>", "user login");
            CommandArgument pass = command.Argument("<password>", "user password");

            command.OnExecute(() =>
            {
                int result = 0;

                if (sname.Value == null || fname.Value == null || pname.Value == null || post.Value == null
                    || depr.Value == null || login.Value == null || pass.Value == null)
                        command.ShowHelp();
                else if (post.Value != "tester" && post.Value != "operator" && post.Value != "admin")
                {
                    Console.WriteLine("Post must be either \"tester\", \"operator\" or \"admin\".");
                    result = -1;
                }
                else
                {
                    switch (Core.AddStaff(sname.Value, fname.Value, pname.Value,
                            post.Value, depr.Value, login.Value, pass.Value))
                    {
                        // TODO: describe
                        case StatusCode.Ok:
                            Logger.Log($"staff-add \"{sname.Value}\" \"{fname.Value}\" " +
                                $"\"{pname.Value}\" {post.Value} {depr.Value}");
                            Console.WriteLine($"Successfully created new staff.");
                            break;

                        default:
                        case StatusCode.Error:
                            break;
                    }
                }
                
                return result;
            });
        }

        /**
         * Команда для исполнения SQL запросов над текущим файлом дампа.
         */
        private static void SqlCommand(CommandLineApplication command)
        {
            Logger Logger = new Logger();
            Logger.Func("Program.SqlCommand");

            command.Description = "Executes given SQL-query using sqlite3.";
            command.HelpOption("-?|-h|--help");

            CommandArgument query = command.Argument("<query-string>", "query to execute inside sqlite3");

            command.OnExecute(() =>
            {
                int result = 0;
                
                if (query.Value == null)
                    command.ShowHelp();
                else
                {
                    switch (Core.ExecSQL(query.Value))
                    {
                        case StatusCode.Ok:
                            Logger.Log($"sql \"{query.Value}\"");
                            break;

                        default:
                        case StatusCode.Error:
                            Console.WriteLine("Error is sql statement!");
                            result = -1;
                            break;
                    }
                }

                return result;
            });
        }
    }
}
