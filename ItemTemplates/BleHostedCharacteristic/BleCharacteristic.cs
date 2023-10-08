using Shiny.BluetoothLE.Hosting;
using Shiny.BluetoothLE.Hosting.Managed;

namespace ShinyApp;


[BleGattCharacteristic(Constants.ManagedServiceUuid, Constants.ManagedCharacteristicUuid)]
public class MyBleGattCharacteristic : BleGattCharacteristic
{
//     public override Task OnStart() => base.OnStart();
//     public override void OnStop() => base.OnStop();

//     public override Task<GattResult> OnRead(ReadRequest request) => base.OnRead(request);
//     public override Task OnWrite(WriteRequest request) => base.OnWrite(request);
//     public override Task OnSubscriptionChanged(IPeripheral peripheral, bool subscribed) => base.OnSubscriptionChanged(peripheral, subscribed);
}
