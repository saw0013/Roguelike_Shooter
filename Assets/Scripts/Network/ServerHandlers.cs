using Mirror;

public struct JumpToScene : NetworkMessage
{
    public Utils.SpawnUtils.PointType pointType;
    public string sceneName;
    public string pointName;
    public string unloadScene;
}

public struct SpawnPlayer : NetworkMessage
{
    public string prefabName;
    public Utils.SpawnUtils.PointType pointType;
    public string sceneName;
    public string pointName;
    public string unloadScene;
}

public struct FinishedMoving : NetworkMessage
{
    public string sceneName;
}

#region Cleint Handles
public struct ClientJumpToScene : NetworkMessage
{
    public Utils.SpawnUtils.PointType pointType;
    public string sceneName;
    public string pointName;
    public int connectionId;
    public string unloadScene;
}
#endregion
