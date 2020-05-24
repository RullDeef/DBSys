using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBSysCore;
using DBSysCore.Model;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace UnitTests
{
    [TestClass]
    public class DBSysCoreTests
    {
        private Staff admin = new Staff()
        {
            surname = "Skulikov",
            firstName = "Aleksey",
            patronymicName = "Vasilievich",
            post = "admin",
            department = "department",
            login = "lexa88",
            password = "xerox"
        };
        private Staff oper = new Staff()
        {
            surname = "Kit",
            firstName = "Tatyana",
            patronymicName = "Sergeevna",
            post = "operator",
            department = "department",
            login = "ledy0K",
            password = "gaga228"
        };
        private Staff tester = new Staff()
        {
            surname = "Vlasov",
            firstName = "Anton",
            patronymicName = "Pavlovich",
            post = "tester",
            department = "department",
            login = "toha1221",
            password = "1221ahota"
        };

        private const string statusTestID = "ID#:4+NZdu5A6RGEphDwBdl4TB";
        private const string nominalTestID = "ID#:GYGggZRj6RGDKcA4lmuzNA";

        private const string dumpFile_1 = "dump";
        private const string dumpFile_2 = "newdump";

        private void CheckFilesAccessory()
        {
            try
            {
                if (File.Exists(Paths.sessionFilename))
                    File.OpenRead(Paths.sessionFilename).Close();
                if (File.Exists(Paths.usersFilename))
                    File.OpenRead(Paths.usersFilename).Close();
                if (File.Exists($"{Paths.dumpsDirectory}//{dumpFile_1}.db"))
                    File.OpenRead($"{Paths.dumpsDirectory}//{dumpFile_1}.db").Close();
                if (File.Exists($"{Paths.dumpsDirectory}//{dumpFile_2}.db"))
                    File.OpenRead($"{Paths.dumpsDirectory}//{dumpFile_2}.db").Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("cant open file.");
                Console.WriteLine(e.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void A00_CleanUp()
        {
            File.Delete(Paths.sessionFilename);
            File.Delete(Paths.usersFilename);
            File.Delete($"{Paths.dumpsDirectory}\\{dumpFile_1}.db");
            File.Delete($"{Paths.dumpsDirectory}\\{dumpFile_2}.db");
        }

        [TestMethod]
        public void A01_SimpleLoginLogout()
        {
            StatusCode status;

            CheckFilesAccessory();

            Core.Status(out string statusStr);
            Console.WriteLine(statusStr);

            status = Core.Login("admin", "dsplab");

            CheckFilesAccessory();

            Core.Status(out statusStr);
            Console.WriteLine(statusStr);

            Assert.AreEqual(StatusCode.Ok, status);

            status = Core.Logout();

            CheckFilesAccessory();

            Assert.AreEqual(StatusCode.Ok, status);
        }

        [TestMethod]
        public void A02_AddStaff_Admin()
        {
            CheckFilesAccessory();

            StatusCode status;
            Core.Login("admin", "dsplab");

            CheckFilesAccessory();

            status = Core.AddStaff(
                admin.surname,
                admin.firstName,
                admin.patronymicName,
                admin.post,
                admin.department,
                admin.login,
                admin.password
            );

            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.GetAllUsers(out List<Staff> users);

            CheckFilesAccessory();

            Assert.AreEqual(StatusCode.Ok, status);
            Assert.AreEqual(1, users.Count);

            Core.Logout();

            CheckFilesAccessory();
        }

        [TestMethod]
        public void A03_AddStaff_Operator()
        {
            CheckFilesAccessory();

            StatusCode status;
            Core.Login("admin", "dsplab");

            CheckFilesAccessory();

            status = Core.AddStaff(
                oper.surname,
                oper.firstName,
                oper.patronymicName,
                oper.post,
                oper.department,
                oper.login,
                oper.password
            );

            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            Core.Logout();

            CheckFilesAccessory();
        }

        [TestMethod]
        public void A04_AddStaff_Tester()
        {
            CheckFilesAccessory();

            StatusCode status;
            Core.Login("admin", "dsplab");

            CheckFilesAccessory();

            status = Core.AddStaff(
                tester.surname,
                tester.firstName,
                tester.patronymicName,
                tester.post,
                tester.department,
                tester.login,
                tester.password
            );

            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            Core.Logout();

            CheckFilesAccessory();
        }

        [TestMethod]
        public void A05_CheckUsersList()
        {
            StatusCode status;
            List<Staff> users;

            CheckFilesAccessory();

            status = Core.GetAllUsers(out users);

            Assert.AreEqual(StatusCode.Ok, status);
            Assert.AreEqual(3, users.Count);

            try
            {
                users.Find(user => user.surname == admin.surname);
                users.Find(user => user.surname == oper.surname);
                users.Find(user => user.surname == tester.surname);
            }
            catch (ArgumentNullException e)
            {
                Assert.Fail("did not found users in users list");
            }

            CheckFilesAccessory();
        }

        [TestMethod]
        public void A06_SwitchDumpFile()
        {
            CheckFilesAccessory();

            StatusCode status;
            status = Core.Login(oper.login, oper.password);

            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.DumpUse(dumpFile_2);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.DumpUse(dumpFile_1);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        [TestMethod]
        public void B01_LoadStaticTests()
        {
            StatusCode status;

            CheckFilesAccessory();

            Console.WriteLine("   LOGGINING IN AS OPERATOR");

            status = Core.Login(oper.login, oper.password);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            Console.WriteLine("   LOADING STATIC TESTS");

            status = Core.LoadStaticTests();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            Console.WriteLine("   LOGGING OUT");

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        // C series - do not logout!
        [TestMethod]
        public void C01_BeginChallenge()
        {
            CheckFilesAccessory();

            StatusCode status;

            // Expect error on attempt from virtual admin
            status = Core.Login(Session.virtualAdmin.login, Session.virtualAdmin.password);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.BeginChallenge(
                "Product Name",
                "CO Name",
                "123",
                "45",
                "v1",
                "CO Parent",
                "Test Challenge #1",
                "location",
                "description"
            );
            Assert.AreEqual(StatusCode.GrantsVirtualAdminNotAllowed, status);

            CheckFilesAccessory();

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Login(oper.login, oper.password);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            // Expect no errors from tester
            status = Core.BeginChallenge(
                "Product Name",
                "CO Name",
                "123",
                "45",
                "v1",
                "CO Parent",
                "Test Challenge #1",
                "location",
                "description"
            );
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        [TestMethod]
        public void C02_DoTests()
        {
            // pick first tests from static.xls and run it

            StatusCode status;

            CheckFilesAccessory();

            status = Core.Test(statusTestID, true);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Test(nominalTestID, true,
                (decimal)5.0, (decimal)4.9, (decimal)0.5, (decimal)0.0);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        [TestMethod]
        public void C03_EndChallenge()
        {
            StatusCode status;

            CheckFilesAccessory();

            status = Core.EndChallenge();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        // D series - challenge with logout on each step - NO!
        [TestMethod]
        public void D01_BeginChallenge()
        {
            StatusCode status;

            CheckFilesAccessory();

            status = Core.Login(admin.login, admin.password);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.DumpUse(dumpFile_2);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Login(tester.login, tester.password);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            // Expect no errors from tester
            status = Core.BeginChallenge(
                "Product Name",
                "CO Name",
                "123",
                "45",
                "v1",
                "CO Parent",
                "Test Challenge #2",
                "location",
                "other description"
            );
            Assert.AreEqual(StatusCode.Ok, status);

            // CheckFilesAccessory();

            // status = Core.Logout();
            // Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        [TestMethod]
        public void D02_DoTests()
        {
            // pick first tests from static.xls and run it

            StatusCode status;

            CheckFilesAccessory();

            // status = Core.Login(tester.login, tester.password);
            // Assert.AreEqual(StatusCode.Ok, status);

            // Console.WriteLine($"  prog state after = {Session.sessionData.programState}");

            // CheckFilesAccessory();

            status = Core.Test(statusTestID, true);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Test(nominalTestID, false,
                (decimal)5.0, (decimal)4.3, (decimal)0.5, (decimal)0.0);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            // status = Core.Logout();
            // Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        [TestMethod]
        public void D03_EndChallenge()
        {
            StatusCode status;

            CheckFilesAccessory();

            // status = Core.Login(tester.login, tester.password);
            // Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.EndChallenge();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        // E series - check test results in dump files
        [TestMethod]
        public void E01_CheckDump_1()
        {
            StatusCode status;

            CheckFilesAccessory();

            status = Core.Login(oper.login, oper.password);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.DumpUse(dumpFile_1);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.GetDynamicTests(out List<TestDynamic> testsList);
            Assert.AreEqual(StatusCode.Ok, status);
            Assert.AreEqual(2, testsList.Count);

            CheckFilesAccessory();

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }

        [TestMethod]
        public void E02_CheckDump_2()
        {
            StatusCode status;

            CheckFilesAccessory();

            status = Core.Login(oper.login, oper.password);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.DumpUse(dumpFile_2);
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();

            status = Core.GetDynamicTests(out List<TestDynamic> testsList);
            Assert.AreEqual(StatusCode.Ok, status);
            Assert.AreEqual(2, testsList.Count);

            CheckFilesAccessory();

            status = Core.Logout();
            Assert.AreEqual(StatusCode.Ok, status);

            CheckFilesAccessory();
        }
    }

    [TestClass]
    public class DBSysCoreTestsSequence
    {
        [TestMethod]
        public void CleanUp()
        {
            DBSysCoreTests tester = new DBSysCoreTests();
            tester.A00_CleanUp();
        }

        [TestMethod]
        public void Main()
        {
            DBSysCoreTests tester = new DBSysCoreTests();

            tester.A00_CleanUp();

            Console.WriteLine("A01_SimpleLoginLogout");
            tester.A01_SimpleLoginLogout();
            Console.WriteLine("A02_AddStaff_Admin");
            tester.A02_AddStaff_Admin();
            Console.WriteLine("A03_AddStaff_Operator");
            tester.A03_AddStaff_Operator();
            Console.WriteLine("A04_AddStaff_Tester");
            tester.A04_AddStaff_Tester();
            Console.WriteLine("A05_CheckUsersList");
            tester.A05_CheckUsersList();
            Console.WriteLine("A06_SwitchDumpFile");
            tester.A06_SwitchDumpFile();

            Console.WriteLine("B01_LoadStaticTests");
            tester.B01_LoadStaticTests();

            Console.WriteLine("C01_BeginChallenge");
            tester.C01_BeginChallenge();
            Console.WriteLine("C02_DoTests");
            tester.C02_DoTests();
            Console.WriteLine("C03_EndChallenge");
            tester.C03_EndChallenge();
            
            tester.D01_BeginChallenge();
            tester.D02_DoTests();
            tester.D03_EndChallenge();
            
            tester.E01_CheckDump_1();
            tester.E02_CheckDump_2();
        }
    }
}
