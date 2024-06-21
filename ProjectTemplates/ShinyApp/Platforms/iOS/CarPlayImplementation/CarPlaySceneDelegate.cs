using System;
using System.Runtime.Versioning;
using CarPlay;
using Foundation;
using UIKit;

namespace ShinyApp.iOS.CarPlayImplementation;


[SupportedOSPlatform("ios16.0")]
[UnsupportedOSPlatform("macos")]
[UnsupportedOSPlatform("maccatalyst")]
[Register("CarPlaySceneDelegate")]
public class CarPlaySceneDelegate : CPTemplateApplicationSceneDelegate
{
    CPInterfaceController controller = null!;
    readonly CPTabBarTemplate tabBarTemplate;
    readonly CPGridTemplate gridTemplate;


	public CarPlaySceneDelegate()
	{
        this.gridTemplate = new CPGridTemplate("Hello World", new CPGridButton[] { })
        {
            TabImage = UIImage.GetSystemImage("circle.grid.3x3.fill")  
        };
        this.tabBarTemplate = new CPTabBarTemplate(new CPTemplate[] { this.gridTemplate });
	}


    public override void DidConnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
    {
        this.controller = interfaceController;
        this.controller.SetRootTemplate(this.tabBarTemplate, true);
    }


    public override void DidDisconnect(CPTemplateApplicationScene templateApplicationScene, CPInterfaceController interfaceController)
    {
        this.controller?.Dispose();
    }
}