using UnityEngine;
using UnityEngine.UI;
using Mirror;
using MyFramework;
using MyFramework.UI;

namespace MySrpg
{

    public class LevelManager_MatchRoom : LevelManager
    {
        /*public float offsetX = 10;
        public float offsetY = 150;

        private void OnGUI()
        {
            if (GUI.Button(new Rect(offsetX, offsetY, 100, 19), "enter"))
            {
                (Game.Instance as SrpgGame).LoadScene("PvP");
            }
        }*/

        public SrpgNetManager netManager;
        public GameObject launchUI;
        public GameObject joinUI;
        public GameObject waitForJoinUI;
        public GameObject enterUI;

        private bool m_isWaitingForConn;

        private void Start()
        {
            HideWaitForJoinUI();

            netManager.networkAddress = "localhost";

            launchUI.GetComponent<Button>().onClick.AddListener(OnLaunchBtnClicked);
            ShowLaunchUI();

            joinUI.GetComponent<Button>().onClick.AddListener(OnJoinBtnClicked);
            ShowJoinUI();


        }

        private void OnLaunchBtnClicked()
        {
            netManager.StartHost();
            NetworkServer.RegisterHandler<Msg_Join>(OnMsgJoint);

            HideLaunchUI();
            HideJoinUI();
            ShowWaitForJoinUI();
        }

        private void OnJoinBtnClicked()
        {
            netManager.StartClient();
            m_isWaitingForConn = true;

            HideLaunchUI();
            HideJoinUI();
        }

        private void OnMsgJoint(NetworkConnection conn, Msg_Join msg)
        {
            Enter();
            HideWaitForJoinUI();
        }

        private void Enter()
        {
            (Game.Instance as SrpgGame).LoadScene("PvP");
        }

        private void OnClientConnect()
        {
            m_isWaitingForConn = false;

            NetworkClient.Send(new Msg_Join());
            Enter();

            HideLaunchUI();
            HideJoinUI();
        }

        private void Update()
        {
            Debug.Log($"ing: {NetworkClient.isConnecting}, ed: {NetworkClient.isConnected}, waiting: {m_isWaitingForConn}");
            if (m_isWaitingForConn && NetworkClient.isConnected)
            {
                OnClientConnect();
            }
        }

        private void ShowLaunchUI()
        {
            launchUI.SetActive(true);
        }

        private void ShowJoinUI()
        {
            joinUI.SetActive(true);
        }

        private void ShowWaitForJoinUI()
        {
            waitForJoinUI.SetActive(true);
        }

        private void HideLaunchUI()
        {
            launchUI.SetActive(false);
        }

        private void HideJoinUI()
        {
            joinUI.SetActive(false);
        }

        private void HideWaitForJoinUI()
        {
            waitForJoinUI.SetActive(false);
        }

    }

}