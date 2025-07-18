using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {
    public ulong clientId;
    public int bodyId;
    public int cloakId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    public bool Equals(PlayerData other) {
        return clientId == other.clientId &&
               bodyId == other.bodyId &&
               cloakId == other.cloakId &&
               playerName == other.playerName &&
               playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref bodyId);
        serializer.SerializeValue(ref cloakId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }
}
