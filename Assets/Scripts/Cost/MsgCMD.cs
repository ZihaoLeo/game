public class MsgCMD
{
    /*
     *  C#中使用4位数字命令
     *  如:1000,1001
     */

    public const int RefreshTxt = 1000;

}
public enum EmDataType
{
    None = 1000000,
    EmStartGameDataRequest              = 1000001,
    EmCreateRoom                        = 1000002,
    EmAddRoom                           = 1000003,
    EmSelectOccupation                  = 1000004,
    EmQuitRoom                          = 1000005,
    EmStartGame                         = 1000007,
    EmSyncShoot                         = 1000008,
    EmSyncStarHealth                    = 1000009,
    EmSyncEnemyPlane                    = 1000010,
    EmSyncGuidedMissile                 = 1000011,
    EmSyncStarShootData                 = 1000012,
    EmSyncPlaneShootData                = 1000013,
    EmSyncGuidedShootData               = 1000014,
    EmSyncShipHealth                    = 1000015,
    EmSyncUpGrade                       = 1000016,
    EmSyncIncreaseFitness               = 1000017,
    EmSyncIncreasedWeaponDamage         = 1000018,
    EmSyncIncreasedWeaponEnchantment    = 1000019,
    EmSyncIncreaseShipSpeed             = 1000020,
    EmSyncWeaponData                    = 1000031,
    EmSyncPlayerData                    = 1000032,
    EmSyncAirshipData                   = 1000033,
    EmSyncSettlementInterface           = 1000034,
}

public enum MessageType
{
    SyncHeartbeat                   = 10000, // heartbeat protocol
    CreateClientId                  = 10001, // After the client connects to the server, the server requests the id of the client, and the server creates the id and name of the client
    CreateRoomData                  = 10002, // Create a room
    AddRoomData                     = 10003, // Join the room
    GetRoomPlayerData               = 10004, // Get player data in the room
    SyncPlayerNameData              = 10005, // Synchronous role name
    SelectPlayerOccupation          = 10006, // Select Player Class
    QuitRoom                        = 10007, // Exit the room
    StartGame                       = 10008, // start game
    SyncShoot                       = 10009, // shoot
    SyncStarHealth                  = 10010, // Synchronize planetary health
    SyncEnemyPlane                  = 10011, // Synchronize ship health
    SyncGuidedMissile               = 10012, // Synchronous missile status
    SyncShipHealth                  = 10013, // Synchronize ship health
    SyncIncreaseFitness             = 10014, // Synchronizing ships increases health
    SyncIncreasedWeaponDamage       = 10015, // Synchronizing ships increases weapon damage
    SyncIncreasedWeaponEnchantment  = 10016, // Sync ships to increase weapon enchantment
    SyncIncreaseShipSpeed           = 10017, // Synchronous ships increase movement speed
    SyncWeaponData                  = 10020, // Synchronous weapon data
    SyncPlayerData                  = 10021, // Synchronize your own role data
    SyncAirshipData                 = 10022, // Synchronize spacecraft position rotation data
    SyncStarShootData               = 10023, // Synchronous planet firing data
    SyncPlaneShootData              = 10024, // Synchronize aircraft firing data
    SyncGuidedData                  = 10025, // Synchronous missile data
    SyncSettlementInterface         = 10030, // Synchronous settlement interface
    ClientOnApplicationQuit         = 10031, // Client exit
    ServerOnApplicationQuit         = 10032, // Server exit
}
