using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deathapillar : MonoBehaviour {

    [Header("Deathapillar Body")]
    [Tooltip("The body of the monster")]
    public Segment[] segments;
    [Tooltip("The segment that leads the rest")]
    public Segment headSegment;
    [Tooltip("How much space should be between each segment")]
    public float distanceBetweenSegments = 50f;

    [Header("Deathapillar Settings")]
    [Tooltip("Reference for creating new body parts")]
    public GameObject segmentPrefab;
    [Tooltip("Reference for when the monster splits")]
    public GameObject deathapillarPrefab;
    [Tooltip("Reference for shooting the laser")]
    public GameObject laserPrefab;
    public float laserSpeed = 10f;
    [Tooltip("Number of segments to create in body")]
    public int numberOfSegments = 20;
    [Tooltip("Min delay for randomizing laser firing")]
    public float laserFiringMinDelay = 1f;
    [Tooltip("Max delay for randomizing laser firing")]
    public float laserFiringMaxDelay = 5f;
    public GameObject attackSpot;

    [Header("Damage Settings")]
    public int collisionDamage = 2;
    public int laserDamage = 1;


    public static Deathapillar originalBody;
    public static Deathapillar oldBody;

    [Header("Health Settings")]
    public float maxHealth;
    public float currentHealth;
    [Tooltip("How much health each segment should have")]
    public float segmentHealth;
    [Header("Journey Settings")]
    [Tooltip("How long it takes to complete from rise to dive")]
    public float journeyTime = 5f;
    [Tooltip("How deep to dive into the ground")]
    public float depth = -30f;
    [Tooltip("How high to go up when rising")]
    public float height = 100f;
    public State state = State.IDLE;
    public float idleTimeMin = 1f;
    public float idleTimeMax = 3f;
    private float idleTime = 2f;
    private float startIdleTime;

    private Parabola parabola;
    private Vector3 positionDivingFrom;
    private Vector3 positionToDive;
    private Vector3 centerPoint;
    private Vector3 startRelCenter;
    private Vector3 endRelCenter;

    private bool HasEnteredGroundFirst = false;

    private Coroutine introCoroutine;
    private Coroutine riseCoroutine;

    [Header("UI Settings")]
    public RectTransform healthBar;
    public Image healthBarFill;

    [Header("Sound Settings")]
    [Tooltip("How loud the sound should be")]
    [Range(0, 1)]
    public float deathIntroVolume = 1f;
    public AudioClip deathIntro;
    [Tooltip("How loud the sound should be")]
    [Range(0, 1)]
    public float deathRiseVolume = 1f;
    public AudioClip deathRise;
    [Tooltip("How loud the sound should be")]
    [Range(0, 1)]
    public float deathDiveVolume = 1f;
    public AudioClip deathDive;
    [Tooltip("How loud the sound should be")]
    [Range(0, 1)]
    public float deathDieVolume = 1f;
    public AudioClip deathDie;
    [Tooltip("How loud the sound should be")]
    [Range(0, 1)]
    public float deathShootVolume = 1f;
    public AudioClip deathShoot;
    [Tooltip("How loud the sound should be")]
    [Range(0, 1)]
    public float deathRumbleVolume = 1f;
    public AudioClip deathRumble;
    [Tooltip("How loud the sound should be")]
    [Range(0, 1)]
    public float deathBreakVolume = 1f;
    public AudioClip deathBreak;

    public enum State {
        CREATION,
        INTRO,
        WAIT,
        IDLE,
        DIVING,
        DEATH
    }

    [System.Serializable]
    public class Segment {
        public bool isHead = false;
        public Segment before;
        public Segment after;

        public GameObject segmentObject;
        public SegmentPart sp;

        public Coroutine diveCoroutine;
        public float startTime;
    }

    public class Parabola {
        float height;
        
        public Parabola(float height) {
            this.height = height;
        }

        public Vector3 Move(Vector3 start, Vector3 end, float height, float t) {
            System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    }

    private void Start() {
        transform.position += new Vector3(0, depth, 0);
        if(originalBody == null) {
            state = State.CREATION;
            originalBody = this;
        } else {
            state = State.IDLE;
        }
        parabola = new Parabola(height);
    }

    // Update is called once per frame
    private void Update() {
        if(originalBody == null) {
            originalBody = this;
            gameObject.name = "Original Body";
            healthBar.gameObject.SetActive(true);
            maxHealth = oldBody.maxHealth;
            currentHealth = oldBody.currentHealth;
            UpdateHealth();
            Destroy(oldBody.gameObject);
            oldBody = null;
        }

        switch (state) {
            case State.CREATION:
                state = State.INTRO;
                maxHealth = segmentHealth * numberOfSegments;
                currentHealth = maxHealth;
                CreateBody(numberOfSegments);
                break;
            case State.IDLE:
                if(Time.time >= startIdleTime + idleTime) {
                    riseCoroutine = StartCoroutine(Rise());
                    state = State.DIVING;
                }
                break;
            case State.DIVING:
                if (!CheckIfDiving() && riseCoroutine == null) {
                    if (CheckDeath()) {
                        state = State.DEATH;
                        break;
                    }
                    CheckSplit();
                    StartIdle();
                }
                break;
            case State.DEATH:
                break;
            case State.INTRO:
                DisableCollisions();
                introCoroutine = StartCoroutine(IntroSequence());
                state = State.WAIT;
                break;
            case State.WAIT:
                if(introCoroutine == null) {
                    healthBar.gameObject.SetActive(true);
                    EnableCollisions();
                    StartIdle();
                }
                break;

        }
    }


    void StartIdle() {
        HasEnteredGroundFirst = false;
        ResetSegments();
        startIdleTime = Time.time;
        idleTime = Random.Range(idleTimeMin, idleTimeMax);
        state = State.IDLE;
    }

    void CreateBody(int i) {
        //CREATE SEGMENTS OF LENGTH i
        segments = new Segment[i];
        //CREATE HEAD
        Segment head = new Segment {
            segmentObject = Instantiate(segmentPrefab),
            isHead = true
        };
        head.sp = head.segmentObject.GetComponent<SegmentPart>();

        MeshRenderer mr = head.sp.GetComponent<MeshRenderer>();
        Material[] newMats = new Material[] { head.sp.headMat };
        mr.materials = newMats;

        head.sp.healthTotal = segmentHealth;
        head.sp.health = segmentHealth;
        head.segmentObject.name = "Head Original";
        segments[0] = head;
        headSegment = head;
        head.segmentObject.transform.position = gameObject.transform.position;
        head.segmentObject.transform.parent = gameObject.transform;

        //CREATE BODY
        Segment current = head;
        for(int s = 1; s < i; s++) {
            Segment body = new Segment {
                segmentObject = Instantiate(segmentPrefab),
                before = current
            };
            body.sp = body.segmentObject.GetComponent<SegmentPart>();
            body.sp.healthTotal = segmentHealth;
            body.sp.health = segmentHealth;
            body.segmentObject.name = s.ToString() + " Original";
            segments[s] = body;
            current.after = body;
            body.segmentObject.transform.position = new Vector3(current.segmentObject.transform.position.x, current.segmentObject.transform.position.y - distanceBetweenSegments, current.segmentObject.transform.position.z);
            body.segmentObject.transform.parent = gameObject.transform;
            current = body;
        }
    }


    void CreateBody(int i, Segment[] newBody) {
        //CREATE SEGMENTS OF LENGTH i
        segments = new Segment[i];
        //CREATE HEAD
        Segment head = new Segment {
            segmentObject = Instantiate(segmentPrefab),
            isHead = true
        };
        Vector3 baseLineSpawn = new Vector3(0, 0, 0);
        head.sp = head.segmentObject.GetComponent<SegmentPart>();
        head.sp.healthTotal = segmentHealth;
        if (newBody[newBody.Length - i].segmentObject == null) {
            Destroy(head.segmentObject.gameObject);
            head.segmentObject = null;
        } else {
            MeshRenderer mr = head.sp.GetComponent<MeshRenderer>();
            Material[] newMats = new Material[] { head.sp.headMat };
            mr.materials = newMats;
            head.sp.health = newBody[newBody.Length - i].sp.health;
            head.segmentObject.name = "New Head";
            head.segmentObject.transform.position = new Vector3(0, depth, 0);
            head.segmentObject.transform.parent = gameObject.transform;
            baseLineSpawn = head.segmentObject.transform.position;
        }
        segments[0] = head;
        headSegment = head;


        //CREATE BODY
        Segment current = head;
        for (int s = 1; s < i; s++) {
            Segment body = new Segment {
                segmentObject = Instantiate(segmentPrefab),
                before = current
            };
            body.sp = body.segmentObject.GetComponent<SegmentPart>();
            segments[s] = body;
            if (newBody[newBody.Length - i + s].segmentObject == null) {
                Destroy(body.segmentObject);
                body.segmentObject = null;
            } else {
                body.sp.healthTotal = segmentHealth;
                body.sp.health = newBody[newBody.Length - i + s].sp.health;
                body.segmentObject.name = s.ToString() + " Split";
                body.segmentObject.transform.position = new Vector3(baseLineSpawn.x, baseLineSpawn.y - (distanceBetweenSegments * s), baseLineSpawn.z);
                body.segmentObject.transform.parent = gameObject.transform;
            }
            current.after = body;
            current = body;
        }
    }


    void CheckSplit() {
        if(segments[0].segmentObject == null) {
            NewHead();
            CheckSplit();
            return;
        }
        if(segments[segments.Length - 1].segmentObject == null) {
            Segment[] tempNew = new Segment[segments.Length - 1];
            for (int y = 0; y < segments.Length - 1; y++) {
                tempNew[y] = segments[y];
            }
            segments = tempNew;
            CheckSplit();
            return;
        }

        for(int i = 1; i < segments.Length; i++) {
            if (segments[i].segmentObject == null) {
                Split(i);
                CheckSplit();
                return;
            }
        }
    }

    void NewHead() {

        bool newHeadFound = false;
        Segment[] temp = new Segment[segments.Length - 1];
        int y = 0;
        for(int i = 1; i < segments.Length; i++) {
            if (newHeadFound) {
                temp[y] = segments[i];
                y++;
                continue;
            }
            if (segments[i].segmentObject != null) {
                segments[i].isHead = true;
                headSegment = segments[i];
                MeshRenderer mr = headSegment.sp.GetComponent<MeshRenderer>();
                Material[] newMats = new Material[] { headSegment.sp.headMat };
                mr.materials = newMats;
                temp = new Segment[segments.Length - i];
                newHeadFound = true;
                temp[y] = segments[i];
                y++;
            }
        }
        if (newHeadFound) {
            segments = temp;
        } else {
            //DIE
        }
    }

    void Split(int i) {
        int length1 = i;
        int length2 = segments.Length - i - 1;
        GameObject g = Instantiate(deathapillarPrefab);
        RoomManager.rm.currentRoom.monsters.Add(g);
        Deathapillar dp = g.GetComponent<Deathapillar>();
        dp.CreateBody(length2, segments);
        dp.CheckSplit();
        Segment[] tempNew = new Segment[length1];
        for(int y = 0; y < length1; y++) {
            tempNew[y] = segments[y];
            tempNew[y].segmentObject.name = y.ToString();
        }
        for(int y = length1; y < segments.Length; y++) {
            Destroy(segments[y].segmentObject);
        }
        segments = tempNew;
    }

    void PlaySound() {

    }

    IEnumerator Rise() {
        //MOVE BASE TO START
        Vector3[] temp = RoomManager.rm.currentRoom.GetTwoRandomNodePosition();
        transform.position = new Vector3(temp[0].x, depth, temp[0].z);

        //MOVE HEAD - SLERP
        positionToDive = transform.position; //head's position
        
        if (this == Deathapillar.originalBody) {
            positionDivingFrom = new Vector3(GameManager.gm.p.transform.position.x, depth, GameManager.gm.p.transform.position.z);
        } else {
            positionDivingFrom = new Vector3(temp[1].x, depth, temp[1].z);
        }

        GameObject attackSpotTemp = Instantiate(attackSpot);
        attackSpotTemp.transform.position = new Vector3(positionDivingFrom.x, 0.75f, positionDivingFrom.z);

        Destroy(attackSpotTemp, 1.5f);
        AudioSource.PlayClipAtPoint(deathRumble, attackSpotTemp.transform.position, deathRumbleVolume);
        yield return new WaitForSeconds(1.5f);

        headSegment.startTime = Time.time;
        headSegment.diveCoroutine = StartCoroutine(RiseEnum(headSegment));

        //MOVE EACH BODY SEGMENT TO THE LAST UNTIL AT THE HEAD'S POSITION
        int i = -1;
        foreach(Segment s in segments) {
            i++;
            if (!s.isHead) {
                s.startTime = Time.time;
                s.diveCoroutine = StartCoroutine(MoveToHeadPosition(s, i));
            }
        }

        AudioSource.PlayClipAtPoint(deathBreak, new Vector3(positionDivingFrom.x, 0.75f, positionDivingFrom.z), deathBreakVolume);
        riseCoroutine = null;
        
    }

    IEnumerator RiseEnum(Segment s) {
        AudioSource.PlayClipAtPoint(deathRise, positionDivingFrom + new Vector3(0, -depth, 0), deathRiseVolume);
        bool soundPlayed = false;
        while (true) {
            float fracComplete = (Time.time - s.startTime) / journeyTime;
            if (s.segmentObject == null) {
                break;
            }
            s.segmentObject.transform.position = parabola.Move(positionDivingFrom, positionToDive, height, fracComplete);
            yield return null;
            if(fracComplete >= 0.8f && !soundPlayed) {
                soundPlayed = true;
                AudioSource.PlayClipAtPoint(deathDive, positionToDive + new Vector3(0, -depth, 0), deathDiveVolume);
            }
            if (fracComplete >= 0.9f && !HasEnteredGroundFirst) {
                AudioSource.PlayClipAtPoint(deathBreak, positionToDive + new Vector3(0, -depth, 0), deathBreakVolume);
                HasEnteredGroundFirst = true;
            }
            if (fracComplete >= 1) {
                break;
            }
        }
        s.diveCoroutine = null;
    }

    IEnumerator MoveToHeadPosition(Segment s, int i) {
        //print("Segment " + i + " position: " + s.segmentObject.transform.position);
        while (true) {
            float fracComplete = (Time.time - s.startTime) / (0.2f * i);
            s.segmentObject.transform.position = Vector3.Lerp(s.segmentObject.transform.position, positionDivingFrom, fracComplete);
            yield return null;
            if(fracComplete >= 1) {
                break;
            }
        }
        yield return new WaitForSeconds(i * 0.2f);
        s.startTime = Time.time;
        s.diveCoroutine = StartCoroutine(RiseEnum(s));
    }
    
    void ResetSegments() {
        int i = 0;
        foreach(Segment s in segments) {
            if(s.segmentObject == null) {
                continue;
            }
            s.segmentObject.transform.localPosition = new Vector3(0, distanceBetweenSegments * i * -1, 0);
            i++;
        }
    }

    void Spit() {

    }

    bool CheckIfDiving() {
        foreach(Segment s in segments) {
            if(s.diveCoroutine != null) {
                return true;
            }
        }
        return false;
    }

    public void UpdateHealth() {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    public bool CheckDeath() {
        foreach(Segment s in segments) {
            if(s.segmentObject != null) {
                return false;
            }
        }
        RoomManager.rm.currentRoom.DefeatMonster(gameObject);
        if (originalBody == this) {
            oldBody = this;
            originalBody = null;
        } else {
            Destroy(gameObject);
        }
        return true;
    }

    IEnumerator IntroSequence() {
        transform.position = new Vector3(GameManager.gm.p.gameObject.transform.position.x + Random.Range(-300f, 300f), depth, GameManager.gm.p.gameObject.transform.position.z + Random.Range(-300f, 300f));

        journeyTime /= 2f;
        yield return new WaitForSeconds(1.5f);
        AudioSource.PlayClipAtPoint(deathBreak, GameManager.gm.player.transform.position + ((transform.position - GameManager.gm.player.transform.position).normalized * 20f), deathBreakVolume);
        
        yield return new WaitForSeconds(1.5f);

        //MOVE HEAD - SLERP
        positionDivingFrom = transform.position; //head's position
        Vector3 temp = RoomManager.rm.currentRoom.roomArray[RoomManager.rm.currentRoom.xLength / 2, RoomManager.rm.currentRoom.yLength / 2].position;
        positionToDive = new Vector3(temp.x, depth, temp.z);

        headSegment.startTime = Time.time;
        headSegment.diveCoroutine = StartCoroutine(RiseEnum(headSegment));

        //MOVE EACH BODY SEGMENT TO THE LAST UNTIL AT THE HEAD'S POSITION
        int i = -1;
        foreach (Segment s in segments) {
            i++;
            if (!s.isHead) {
                s.startTime = Time.time;
                s.diveCoroutine = StartCoroutine(MoveToHeadPosition(s, i));
            }
        }

        //yield return new WaitForSeconds(1.3f);
        GameManager.gm.p.audioSource.PlayOneShot(deathIntro, deathIntroVolume);

        while (CheckIfDiving()) {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        journeyTime *= 2f;
        introCoroutine = null;
    }

    void EnableCollisions() {
        foreach (Segment s in segments) {
            if (s.segmentObject == null) {
                continue;
            }
            BoxCollider bc = s.segmentObject.GetComponent<BoxCollider>();
            bc.isTrigger = false;
        }
    }

    void DisableCollisions() {
        foreach(Segment s in segments) {
            if(s.segmentObject == null) {
                continue;
            }
            BoxCollider bc = s.segmentObject.GetComponent<BoxCollider>();
            bc.isTrigger = true;
        }
    }


}
