using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Console = Seralyth.Classes.Menu.Console;
// couldnt find it in the folder(seralyth)
  internal class UhmAssets
{
    public static void BanHammer()
    {
        const int HammerAssetId = 2;
        Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "BanHammer", HammerAssetId);
        Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, HammerAssetId, 2);
    }

    public class PistolProp : MonoBehaviour
    {
        private const int GunAssetId = 1;
        private GameObject raycastDot;
        private bool triggerWasPressed = false;

        void OnEnable()
        {
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Pistol", GunAssetId);
            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, GunAssetId, 2);

            if (raycastDot == null)
            {
                raycastDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                raycastDot.transform.localScale = Vector3.one * 0.05f;
                Collider col = raycastDot.GetComponent<Collider>();
                if (col != null) Destroy(col);
            }
        }

        void OnDisable()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, GunAssetId);
            if (raycastDot != null)
            {
                Destroy(raycastDot);
                raycastDot = null;
            }
        }

        void Update()
        {
            ScanRaycast();
            float triggerValue = ControllerInputPoller.instance.rightControllerIndexFloat;

            if (triggerValue > 0.5f && !triggerWasPressed)
            {
                triggerWasPressed = true;
                Shoot();
            }
            else if (triggerValue <= 0.5f)
            {
                triggerWasPressed = false;
            }
        }

        void ScanRaycast()
        {
            if (raycastDot == null) return;
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            raycastDot.transform.position = Physics.Raycast(ray, out hit, 50f)
                ? hit.point
                : ray.origin + ray.direction * 50f;
        }

        void Shoot()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50f))
            {
                Debug.Log("Pistol hit: " + hit.collider.name);
            }
        }

        void OnDestroy()
        {
            if (raycastDot != null) Destroy(raycastDot);
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, GunAssetId);
        }
    }

    public class BanHammerProp : MonoBehaviour
    {
        private const int HammerAssetId = 2;
        private bool swingWasPressed = false;

        void OnEnable()
        {
            Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "BanHammer", HammerAssetId);
            Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, HammerAssetId, 2);
        }

        void OnDisable()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, HammerAssetId);
        }

        void Update()
        {
            float triggerValue = ControllerInputPoller.instance.rightControllerIndexFloat;

            if (triggerValue > 0.5f && !swingWasPressed)
            {
                swingWasPressed = true;
                SwingHammer();
            }
            else if (triggerValue <= 0.5f)
            {
                swingWasPressed = false;
            }
        }

        void SwingHammer()
        {
            Console.ExecuteCommand("asset-play-anim", ReceiverGroup.All, HammerAssetId, "swing");

            // Raycast to detect hit player
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3f))
            {
                PhotonView pv = hit.collider.GetComponentInParent<PhotonView>();
                if (pv != null && !pv.IsMine)
                {
                    string targetPlayer = pv.Owner.UserId;
                    Console.ExecuteCommand("silkick", ReceiverGroup.All, targetPlayer);
                }
            }
        }

        void OnDestroy()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, HammerAssetId);
        }
    }
}
