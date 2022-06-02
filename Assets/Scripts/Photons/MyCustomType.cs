using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photons
{
    public static class MyCustomType
    {
        public static readonly byte[] bufferVector2Int = new byte[8];

        public static void Register()
        {
            PhotonPeer.RegisterType(typeof(Vector2Int), 2, SerializeVector2Int, DeserializeVector2Int);
        }

        private static short SerializeVector2Int(StreamBuffer outStream, object customObject)
        {
            Vector2Int v = (Vector2Int)customObject;
            int index = 0;
            lock (bufferVector2Int)
            {
                Protocol.Serialize(v.x, bufferVector2Int, ref index);
                Protocol.Serialize(v.y, bufferVector2Int, ref index);
                outStream.Write(bufferVector2Int, 0, index);
            }
            return (short)index;
        }

        private static object DeserializeVector2Int(StreamBuffer inStream, short length)
        {
            int x, y;
            int index = 0;
            lock (bufferVector2Int)
            {
                inStream.Read(bufferVector2Int, 0, length);
                Protocol.Deserialize(out x, bufferVector2Int, ref index);
                Protocol.Deserialize(out y, bufferVector2Int, ref index);
            }
            return new Vector2Int(x, y);
        }
    }
}

