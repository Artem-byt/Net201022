using ExitGames.Client.Photon;
using System;
using System.Linq;
using UnityEngine;

public class PhotonRegister : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonPeer.RegisterType(typeof(CastHit), 242, SerializeCastHit, DeserializeCastHit);
    }

    public static object DeserializeCastHit(byte[] data)
    {
        CastHit result = new CastHit();
        result.Direction = new Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
        result.Id = BitConverter.ToSingle(data, 12);

        return result;
    }

    public static byte[] SerializeCastHit(object obj)
    {
        CastHit castHit = (CastHit)obj;

        byte[] result = new byte[16];
        BitConverter.GetBytes(castHit.Direction.x).CopyTo(result, 0);
        BitConverter.GetBytes(castHit.Direction.y).CopyTo(result, 4);
        BitConverter.GetBytes(castHit.Direction.z).CopyTo(result, 8);
        BitConverter.GetBytes(castHit.Id).CopyTo(result, 12);

        return result;
       
    }
}
