namespace Voodoo.Tiny.Sauce.Internal.ABTest
{
    public interface IABTestManager
    {
        void Init();
        string[] GetAbTestValues();
    }
}
