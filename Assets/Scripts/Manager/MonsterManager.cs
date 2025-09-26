using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using static Enums;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] Player PLAYER;

    List<GameObject> monster_ResourseObj = new List<GameObject>();
    List<Monster_Base> monster_Summon_List = new List<Monster_Base>();

    Dictionary<Monster_Base, GameObject> monsterDatas = new Dictionary<Monster_Base, GameObject>();

    public List<Monster_Base> MonsterList = new List<Monster_Base>();//Creat monster List
    //public List<GameObject> MonsterObjectList = new List<GameObject>();//Creat monster List
    public List<GameObject> SummonMonsterList = new List<GameObject>();

    [SerializeField] Transform Creatab;
    [SerializeField] StateBar HPBAR;

    GameObject CanvasObj;
    bool isUiOpen = false;
    bool isSpawn = false;
    public bool isFade = false;

    Dictionary<int, info> stageMonsterData = new Dictionary<int, info>();
    info nowStageinfo = null;
    public List<Monster_Base> NowStagMonterList = new List<Monster_Base>();
    float SpownTime = 2.0f;

    [Header("DeathEffect")]
    GameObject nomalDeathEffectObj;
    GameObject bossDeathEffectObj;
    [Header("SpownEffect")]
    GameObject nomalSpownEffectObj;
    GameObject bossSpownEffectObj;
    BossStateBar BossStateUi;

    public event Action<Monster_Base> OnMonsterDied;

    class info
    {
        public CHARACTER_ID NomalId;
        public CHARACTER_ID BossId;
        public Queue<NomalMonster> nomalData = new Queue<NomalMonster>();
        public NomalMonster nomal = null;
        public BossMonster boss = null;
    }

    public async UniTask InitAsync()
    {
        GameShard.Instance.MonsterManager = this;

        bossDeathEffectObj = Shared.Instance.ResourcesManager.bossDeathEffect;
        nomalDeathEffectObj = Shared.Instance.ResourcesManager.nomalDeathEffect;

        bossSpownEffectObj = Shared.Instance.ResourcesManager.SpownEffect;
        nomalSpownEffectObj = Shared.Instance.ResourcesManager.SpownEffect;

        MonsterSetttig();

        await UniTask.Yield(); // 필요시 프레임 분산
    }

    public void CharacterActive(bool paused)
    {
        if (paused == true)
        {
            isSpawn = false;
        }

        else
        {
            isSpawn = true;
            StartCoroutine(SpawnStart(nowStageinfo, SpownTime));
        }

        for (int i = NowStagMonterList.Count - 1; i >= 0; i--)
        {
            var m = NowStagMonterList[i];
            if (m == null) 
            { 
                NowStagMonterList.RemoveAt(i); 
                continue; 
            }

            try 
            { 
                m.StateUpdate(paused);
            }
            
            catch { }

            var anim = m.GetComponentInChildren<Animator>();
        }
    }

    public void UiStateUpdate(bool _state)
    {
        CleanNowStageList();
        CharacterActive(_state);
        isUiOpen = _state;
    }

    private void CleanNowStageList()
    {
        if(NowStagMonterList.Count ==0) return;

        for (int i = NowStagMonterList.Count - 1; i >= 0; i--)
            if (NowStagMonterList[i] == null)
                NowStagMonterList.RemoveAt(i);
    }
    public void ApplyCanvas(GameObject _canvas) 
    {
        CanvasObj = _canvas;
        BossStateUi = CanvasObj.GetComponentInChildren<BossStateBar>(true);
        BossStateUi.gameObject.SetActive(false);
        //BossStateUi.gameObject.SetActive(false);
    }
 
    private Vector3 GetPoisotion(Bounds bounds,GameObject _obj)
    {
        Vector3 targetPos = new Vector3
            (PLAYER.transform.position.x,
            1.0f,
            PLAYER.transform.position.z
            );

        float randomX = UnityEngine.Random.Range(targetPos.x - 20f, targetPos.x + 20f);
        float randomZ = UnityEngine.Random.Range(targetPos.z - 20f, targetPos.z + 20f);
        //float randomX = UnityEngine.Random.Range(targetPos.x - 10f, targetPos.x + 10f);
        // float randomZ = UnityEngine.Random.Range(targetPos.z - 10f, targetPos.z + 10f);

        float clampedX = Mathf.Clamp(randomX, bounds.min.x, bounds.max.x);
        float clampedZ = Mathf.Clamp(randomZ, bounds.min.z, bounds.max.z);

        Vector3 spawnXZ = new Vector3(clampedX, 200f, clampedZ); // 임시로 높이 위쪽
        Ray ray = new Ray(spawnXZ, Vector3.down);

        LayerMask ground = LayerMask.GetMask(LAYER_TYPE.Walkable.ToString());

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ground)) 
        {
            if (NavMesh.SamplePosition(hitInfo.point, out var navHit, 2f, NavMesh.AllAreas))
            {
                NavMeshAgent agent = _obj.GetComponent<NavMeshAgent>();
                agent.Warp(navHit.position); // NavMesh 위로 순간이동
            }



            return new Vector3(clampedX, (float)hitInfo.point.y, clampedZ);
        }
        return new Vector3(clampedX, targetPos.y, clampedZ);
    }
    public void Resurrection(Monster_Base _monster)
    {
        //GameObject monsterObj = monsterDatas[_monster];
        //monsterObj.SetActive(false);
        if (_monster is BossMonster) 
        {
            BossStateUi.gameObject.SetActive(false);
            return;
        }

        _monster.gameObject.SetActive(false);

        if (_monster.IdType == nowStageinfo.NomalId)
        {
            nowStageinfo.nomalData.Enqueue(_monster as NomalMonster);

            if (!isSpawn)
            {
                StartCoroutine(SpawnStart(nowStageinfo, SpownTime));
            }
        }
        else 
        {
            Destroy(_monster.gameObject);
        }     
    }

    public void MonsterAcquired(Monster_Base m)
    {
        OnMonsterDied?.Invoke(m);
    }


    public void PlayerInit(Player _player)
    {
        PLAYER = _player;
    }
   
    public void StageMonsterCreat(STAGE _stageId)//Stage Manager
    {
        if (stageMonsterData.TryGetValue((int)_stageId, out info info))
        {
            if (nowStageinfo != null)
            {
                nowStageinfo = null;
                for (int i = NowStagMonterList.Count - 1; i >= 0; i--)
                {
                    if (NowStagMonterList[i] != null)
                    {
                        Destroy(NowStagMonterList[i]);
                    }
                    NowStagMonterList.RemoveAt(i);
                }
            }
            else { isSpawn = true; }


            if (Creatab == null)
            {
                GameObject go = new GameObject("MonsterTab");
                Creatab = go.transform;
            }

            nowStageinfo = info;

            int count = 50;
            
            for (int i = 0; i < count; i++) 
            {
                Monster_Base monster1 = CreatMonsterObject(info.nomal.gameObject, null, Creatab);
                nowStageinfo.nomalData.Enqueue(monster1 as NomalMonster);
            }

            Monster_Base monster2 = CreatMonsterObject(info.boss.gameObject, null, Creatab);
            nowStageinfo.boss = monster2 as BossMonster;

        }

    }

    public void SpawnActive() 
    {
        StartCoroutine(SpawnStart(nowStageinfo, SpownTime));
    }

    public async UniTask BossSpawn()
    {
        GameObject go = GameObject.Find("Ground");
        BoxCollider boxCollider = go.GetComponent<BoxCollider>();
        Bounds bounds = boxCollider.bounds;

        if (nowStageinfo != null)
        {
            //isSpawn = false;

            BossMonster boss = nowStageinfo.boss;

            boss.IsPaused = false;

            BossStateUi.gameObject.SetActive(true);

            if (!boss.gameObject.activeSelf)
            {
                boss.gameObject.SetActive(true);
            }

            boss.transform.position = GetPoisotion(bounds,boss.gameObject);
            boss.StateReset();

            NowStagMonterList.Add(boss);

            await Shared.Instance.SoundManager.BgmPlayerSetting((int)BGM_ID_TYPE.Boss);
        }
    }

    private IEnumerator SpawnStart(info info, float _spownDelay)
    {
        GameObject go = GameObject.Find("Ground");
        BoxCollider boxCollider = go.GetComponent<BoxCollider>();
        Bounds bounds = boxCollider.bounds;

        while (isSpawn)
        {
            yield return new WaitWhile(() => isUiOpen);

            yield return new WaitForSeconds(_spownDelay);

            if (!isSpawn)
            {
                yield break; // 코루틴 완전히 종료
            }

            if (info.nomalData.Count > 0)
            {

                for (int i = 2; i > 0; i--)
                {
                    if (info.nomalData.Count == 0) 
                    {
                        break;
                    }

                    NomalMonster firstmoster = info.nomalData.Dequeue();
                    firstmoster.IsPaused = false;

                    if (firstmoster == null) { continue; }

                    if (!firstmoster.gameObject.activeSelf)
                    {
                        firstmoster.gameObject.SetActive(true);
                    }

                    Rigidbody rg = firstmoster.gameObject.GetComponent<Rigidbody>();
                    if (rg != null)
                    {
                        rg.linearVelocity = Vector3.zero;
                        rg.angularVelocity = Vector3.zero;
                    }

                    firstmoster.transform.position = GetPoisotion(bounds, firstmoster.gameObject);
                    firstmoster.StateReset();

                    NowStagMonterList.Add(firstmoster);
                }

            }
            //else
            //{
            //    //monsterSpownEvnet = null;//코루틴
            //    yield break;
            //}

            //boss.IsPaused = false;
            //boss.gameObject.SetActive(true);

        }
        yield return null;
    }
   

    public void MonsterSetttig()
    {
        //CanvasObj = Shared.Instance.ResourcesManager.CanvasObj;

        monster_ResourseObj = Shared.Instance.ResourcesManager.character_ResourseObj;

        Dictionary<int, Monster_Base> CharcterData = new Dictionary<int, Monster_Base>();

        for (int i = 0; i < monster_ResourseObj.Count; i++) 
        {
            Monster_Base monster = monster_ResourseObj[i].GetComponent<Monster_Base>();

            int id = (int)monster.IdType;

            CharcterData.Add(id, monster);
        }

        int count = Shared.Instance.DataManager.Stage_Table.StageTableData.Count;

        for (int i = 0; i < count; i++) 
        {
            info info = new info();

            var data = Shared.Instance.DataManager.Stage_Table.Get(i+1);

            if (data == null) { break; }

            int nomalId = data.Monster_1Id;
            int bossId = data.Boss_1Id;


            if (CharcterData.TryGetValue(nomalId,out Monster_Base nomal_base)) 
            {
                //NomalMonster nomal = nomal_base as NomalMonster;

                //nomal = CreatMonsterObject(nomal.gameObject, null, Creatab) as NomalMonster;// 30<- table Data
                info.nomal = nomal_base as NomalMonster;
                info.NomalId = nomal_base.IdType;
                
            }
            else { Debug.LogError($"{nomalId} = null"); }


            if (CharcterData.TryGetValue(bossId, out Monster_Base boss_base))
            {
                //BossMonster boss = boss_base as BossMonster;

                //boss = CreatMonsterObject(boss.gameObject, null, Creatab) as BossMonster;
                info.boss = boss_base as BossMonster;
                info.BossId = boss_base.IdType;
            }
            else { Debug.LogError($"{bossId} = null"); }


            stageMonsterData.Add(i+1, info);
        }

        Debug.Log($"{stageMonsterData}");
    }

    public Monster_Base CreatMonsterObject(GameObject _resorseObj, Spawn _spown,Transform _parent)
    {
        Vector3 pos = new Vector3(0,0.3f,0);
        if (_spown != null) 
        {
            pos = _spown.transform.position;
        } 

        GameObject monsterObj = Instantiate(_resorseObj, pos, Quaternion.identity, _parent);

        Monster_Base monster = monsterObj.GetComponent<Monster_Base>();

        monster.PlayerInit(PLAYER, Creatab);

        monster.GetData();

        BasicSkillSystem skillSystem = new BasicSkillSystem();

        skillSystem.Init(monster);
        monster.SkillInit(skillSystem);

        ParticleSystem deathEffect = null;
        ParticleSystem SpownEffect = null;

        if (monster is NomalMonster)
        {
            StateBar hpBar = monsterObj.GetComponentInChildren<StateBar>();
            hpBar.InitializeCharacter(monster);
            monster.HpBarInit(hpBar);

            GameObject go1 = Instantiate(nomalDeathEffectObj, Vector3.zero, Quaternion.identity ,monster.transform);
            AudioSource audioSource1 = go1.AddComponent<AudioSource>();
            audioSource1 = Shared.Instance.SoundManager.SoundSetting(10031, audioSource1);

            deathEffect = go1.GetComponent<ParticleSystem>();
            go1.SetActive(false);

            GameObject go2 = Instantiate(nomalSpownEffectObj, Vector3.zero, Quaternion.identity, monster.transform);
            AudioSource audioSource2 = go2.AddComponent<AudioSource>();
            audioSource2 = Shared.Instance.SoundManager.SoundSetting(10031, audioSource2);

            SpownEffect = go2.GetComponent<ParticleSystem>();

            go2.SetActive(false);
        }
        else 
        {
            if (monster is BossMonster)
            {
                GameObject go = Instantiate(bossDeathEffectObj,Vector3.zero,Quaternion.identity ,monster.transform);
                AudioSource audioSource = go.AddComponent<AudioSource>();
                audioSource = Shared.Instance.SoundManager.SoundSetting(10032, audioSource);

                deathEffect = go.GetComponent<ParticleSystem>();
                go.SetActive(false);

                GameObject go2 = Instantiate(bossSpownEffectObj, Vector3.zero, Quaternion.identity, monster.transform);
                AudioSource audioSource2 = go2.AddComponent<AudioSource>();
                audioSource2 = Shared.Instance.SoundManager.SoundSetting(10031, audioSource2);

                SpownEffect = go2.GetComponent<ParticleSystem>();
                go2.SetActive(false);

                if (CanvasObj == null)
                {
                    Debug.LogError($"CanvasObj = {CanvasObj}");
                }
                else 
                {
                    if (BossStateUi != null)
                    {
                        monster.HpBarInit(BossStateUi);
                        BossStateUi.InitializeCharacter(monster);
                        BossStateUi.gameObject.SetActive(false);
                    }
                }
                
            }
            else 
            {
                Debug.LogError($"monster = {monster}");
            }
        }
        deathEffect.Stop();
        monster.deathEffect = deathEffect;
        monster.SpownEffect = SpownEffect;

        //if (_spown != null)
        //{
        //    monster.AutoMoveInit(_spown);
        //}


        monsterObj.SetActive(false);
        monsterDatas.Add(monster, monsterObj);
        MonsterList.Add(monster);

        //ParticleSystem deathEfect = Shared.Instance.ResourcesManager.CreatObject(Skill, skillData.prefabPath);
        //monster.deathEfect = deathEfect;
        //Item
        //Shared.ItemManager.ItemDataAddToMonster(monster);

        return monster;
    }

    public void DespawnAllEnemies(bool includeBoss = true)
    {
        CharacterActive(true);

        for (int i = NowStagMonterList.Count - 1; i >= 0; i--)
        {
            var m = NowStagMonterList[i];
            if (m == null)
            {
                NowStagMonterList.RemoveAt(i);
                continue;
            }

            if (!includeBoss && m is BossMonster)
                continue;

            // 풀을 쓰면 ReturnToPool 권장
            Destroy(m.gameObject);
            NowStagMonterList.RemoveAt(i);
        }

        // 보스 UI 정리
        if (BossStateUi != null) BossStateUi.gameObject.SetActive(false);
    }



    //public List<Monster_Base> CreatMonster(Spawn[] _spowns, Transform _parent)
    //{
    //    GameObject go = null;
    //    List<Monster_Base> monsterList = new List<Monster_Base>();
    //    foreach (Spawn t in _spowns) //Spown
    //    {
    //        foreach (var monsterObj in monster_ResourseObj)
    //        {
    //            Monster_Base monster1 = monsterObj.gameObject.GetComponent<Monster_Base>();

    //            if (t.SpownMonsterType == monster1.IdType)
    //            {
    //                go = monsterObj;
    //                //CreatMonsterObject(go, t, _parent);
    //                monsterList.Add(CreatMonsterObject(go, t, _parent));
    //                go = null;
    //            }
    //            //else if (monster1.Type == CHARACTER_TYPE.Nano)
    //            //{
    //            //    MonsterObj_Minan = monsterObj;
    //            //    //go = null;
    //            //}
    //            else
    //            {
    //                go = null;
    //            }
    //        }

    //    }
    //    //End
    //    return monsterList;
    //}

    //public void creatMinian(int _count, GameObject _obj)
    //{
    //    List<GameObject> minanObjs = new List<GameObject>();

    //    for (int i = 0; i < _count; i++)
    //    {
    //        GameObject monsterObj = Instantiate(_obj, Minan_Master_Pos, Quaternion.identity, Creatab);
    //        monsterObj.SetActive(false);

    //        minanObjs.Add(monsterObj);
    //    }
    //    Reverse reverse = Revers_monionMaster.GetComponent<Reverse>();
    //    reverse.MnianList(minanObjs);
    //}


    //private void Spown()
    //{
    //    Room[] room = Room.FindObjectsByType<Room>(FindObjectsSortMode.None);

    //    for (int i = 0; i < room.Length; i++)
    //    {
    //        Spawn[] Spawn = room[i].transform.GetComponentsInChildren<Spawn>(true);

    //        room[i].inMonsterLists = CreatMonster(Spawn, Creatab);
    //        room[i].init();
    //        GameShard.Instance.GameManager.roomData.Add(room[i], room[i].gameObject);
    //        GameShard.Instance.GameManager.rooms.Add(room[i]);
    //    }
    //}


}
