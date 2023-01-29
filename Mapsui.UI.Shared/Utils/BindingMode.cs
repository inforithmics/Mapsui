namespace Mapsui.UI.Utils
{
#if __ANDROID__ || __IOS__ || __ETO_FORMS__
    public enum BindingMode
    {
        OneWay,
        TwoWay
    }
#endif    
}