namespace UnitTest2012Ultimate.API.Utilities
{
    using SpendManagementLibrary;

    public class MockAccount : cAccount
    {
        public new int accountid {
            get { return GlobalTestVariables.AccountId; }
        }
        public new bool ValidationServiceEnabled { get { return true; } }
    }
}
