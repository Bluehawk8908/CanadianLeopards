using UnityEngine;
using GHPC;
using GHPC.Vehicle;


namespace CanadianLeopards
{
    class Leo2
    {
        public static void Convert(Vehicle vehicle)
        {
            GameObject vehicle_go = vehicle.gameObject;
            vehicle_go.AddComponent<CanLepConverted>();
            Utilities.Log("Leopard 2 found - nothing done to it yet");
        }
    }
}
