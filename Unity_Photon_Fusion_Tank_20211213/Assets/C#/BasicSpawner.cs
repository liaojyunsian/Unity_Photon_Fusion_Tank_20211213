using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//�ޥ� Fusion �R�W�Ŷ�
using Fusion;
using Fusion.Sockets;
using System;

//INetworkRunnerCallbacks �s�u���澹�^�I�����DRunner ���澹�B�z�欰��|�^�I����������k
/// <summary>
/// �s�u�򩳥ͦ���
/// </summary>
public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    #region ��

    #endregion

    #region ���

    [Header("�ЫػP�[�J�ж����")]
    public InputField inputFieldCreateRoom;
    public InputField inputFieldJoinRoom;
    [Header("���a����� - �s�u�w�s��")]
    public NetworkPrefabRef goPlayer;
    [Header("���a����� - �s�u�w�s��")]
    public GameObject goCanvas;
    [Header("������r")]
    public Text textVersion;

    /// <summary>
    /// ���a��J���ж��W��
    /// </summary>
    private string roomNameInput;
    /// <summary>
    /// �s�u���澹
    /// </summary>
    private NetworkRunner runner;

    private string stringVesion = "������ : ";

    #endregion

    private void Awake()
    {
        textVersion.text = stringVesion + Application.version;
    }

    #region ��k

    /// <summary>
    /// ���s�I���I�s�Q�Ыةж�
    /// </summary>
    public void BtnCreateRoom()
    {
        roomNameInput = inputFieldCreateRoom.text;

        print("�Ыةж� " + roomNameInput);

        StartGame(GameMode.Host);
    }

    /// <summary>
    /// ���s�I���I�s�Q�[�J�ж�
    /// </summary>
    public void BtnJoinRoom()
    {
        roomNameInput = inputFieldJoinRoom.text;

        print("�[�J�ж� " + roomNameInput);

        StartGame(GameMode.Client);
    }

    //async �D�P�B�B�z�Q����t�ήɳB�z�s�u
    /// <summary>
    /// �}�l�s�u�C��
    /// </summary>
    /// <param name="mode">�s�u�Ҧ��Q�D���B�Ȥ�</param>
    private async void StartGame(GameMode mode)
    {
        print("<color=yellow>�}�l�s�u</color>");

        //�s�u���澹 = �K�[����<�s�u���澹>
        runner = gameObject.AddComponent<NetworkRunner>();
        //�s�u���澹.�O�_���ѿ�J = �O
        runner.ProvideInput = true;

        //���ݳs�u�Q�C���s�u�Ҧ��B�ж��W�١B�s�u�᪺�����B�����޲z��
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomNameInput,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        print("<color=yellow>�s�u����</color>");
        goCanvas.SetActive(false);
    }

    #region Fusion �^�G�禡�ϰ� ��@ INetworkRunnerCallbacks

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    /// <summary>
    /// ���a�s�u��J�欰
    /// </summary>
    /// <param name="runner">�s�u���澹</param>
    /// <param name="input">��J��T</param>
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

        NetworkInputData inputData = new NetworkInputData();

        #region �ۭq��J����P���ʸ�T

        //W �e
        if (Input.GetKey(KeyCode.W))
        {
            inputData.dierction += Vector3.forward;
        }

        //S ��
        if (Input.GetKey(KeyCode.S))
        {
            inputData.dierction += Vector3.back;
        }

        //A ��
        if (Input.GetKey(KeyCode.A))
        {
            inputData.dierction += Vector3.left;
        }

        //D �k
        if (Input.GetKey(KeyCode.D))
        {
            inputData.dierction += Vector3.right;
        }

        //�ƹ�����o�g
        inputData.inputFire = Input.GetKey(KeyCode.Mouse0);

        #endregion

        #region �ƹ��y�гB�z
        //���o �ƹ��y��
        inputData.positionMouse = Input.mousePosition;
        //�]�w �ƹ��y�� Z �b - �i�H���� 3D ����D�j����v����Y
        inputData.positionMouse.z = 60;

        //�z�L API �N�ƹ��ର�@�ɮy��
        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(inputData.positionMouse);
        //�x�s�ഫ�᪺�ƹ��y��
        inputData.positionMouse = mouseToWorld;
        #endregion

        //��J��T.�]�w(�s�u��J���)
        input.Set(inputData);

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    [Header("�ЫػP�[�J�ж����")]
    public Transform[] traSpawnPoints;

    /// <summary>
    /// ���a��ƶ��X�Q���a�ѦҸ�T�D���a�s�u����
    /// </summary>
    private Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();

    /// <summary>
    /// ���a���\�[�J�ж���
    /// </summary>
    /// <param name="runner">�s�u���澹</param>
    /// <param name="player">���a��T</param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //�H���ͦ��I = Unity �� �H���d��(0�D�ͦ���m�ƶq)
        int randomSpawnPoint = UnityEngine.Random.Range(0, traSpawnPoints.Length);
        //�s�u���澹.�ͦ�(����D�y�СD���סD���a��T)
        NetworkObject playerNetworkObject = runner.Spawn(goPlayer, traSpawnPoints[randomSpawnPoint].position, Quaternion.identity, player);
        //�N���a�ѦҸ�T�P���a�s�u����K�[��r�嶰�X��
        players.Add(player, playerNetworkObject);
    }

    /// <summary>
    /// ���a���}�ж���
    /// </summary>
    /// <param name="runner">�s�u���澹</param>
    /// <param name="player">���a��T</param>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //�p�G ���}�����a�s�u���� �s�b �N�R��
        if (players.TryGetValue(player, out NetworkObject playerNetworkObject))
        {
            //�s�u���澹.�����ͦ�(�Ӫ��a�s�u���󲾰�)
            runner.Despawn(playerNetworkObject);
            //���a���X.����(�Ӫ��a)
            players.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    #endregion

    #endregion

}
