using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deathapillar : MonoBehaviour {

    public Segment[] segments;
    public Segment headSegment;
    [Header("Deathapillar Settings")]
    public GameObject segmentPrefab;

    public GameObject deathapillarPrefab;
    public int numberOfSegments = 20;

    public static Deathapillar originalBody;

    public float maxHealth;
    public float currentHealth;
    public float distanceBetweenSegments = 50f;
    public float journeyTime = 5f;
    public float depth = -30f;
    public float height = 100f;
    public State state = State.IDLE;
    public float idleTime = 3f;
    private float startIdleTime;

    private Parabola parabola;
    public Vector3 positionDivingFrom;
    private Vector3 positionToDive;
    private Vector3 centerPoint;
    private Vector3 startRelCenter;
    private Vector3 endRelCenter;

    public float segmentHealth;
    public RectTransform healthBar;
    public Image healthBarFill;

    public enum State {
        CREATION,
        IDLE,
        DIVING
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
        //CreateBody(25);
        //positionToDive = new Vector3(GameManager.gm.p.transform.position.x, depth, GameManager.gm.p.transform.position.z);
        //positionToDive = GameManager.gm.p.transform.position;
        //startIdleTime = Time.time;
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

        switch (state) {
            case State.CREATION:
                state = State.IDLE;
                healthBar.gameObject.SetActive(true);
                maxHealth = segmentHealth * numberOfSegments;
                currentHealth = maxHealth;
                CreateBody(numberOfSegments);
                break;
            case State.IDLE:
                if(Time.time >= startIdleTime + idleTime) {
                    Rise();
                    state = State.DIVING;
                }
                break;
            case State.DIVING:
                if (!CheckIfDiving()) {
                    CheckSplit();
                    ResetSegments();
                    startIdleTime = Time.time;
                    state = State.IDLE;
                }
                break;
        }

        //Rise();
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
        head.sp = head.segmentObject.GetComponent<SegmentPart>();
        head.sp.healthTotal = segmentHealth;
        head.sp.health = newBody[newBody.Length - i].sp.health;
        head.segmentObject.name = "New Head";
        segments[0] = head;
        headSegment = head;
        head.segmentObject.transform.position = gameObject.transform.position;
        head.segmentObject.transform.parent = gameObject.transform;

        //CREATE BODY
        Segment current = head;
        for (int s = 1; s < i; s++) {
            Segment body = new Segment {
                segmentObject = Instantiate(segmentPrefab),
                before = current
            };
            body.sp = body.segmentObject.GetComponent<SegmentPart>();
            segments[s] = body;
            body.sp.healthTotal = segmentHealth;
            body.sp.health = newBody[newBody.Length - i + s].sp.health;
            body.segmentObject.name = s.ToString() + " Split";
            current.after = body;
            body.segmentObject.transform.position = new Vector3(current.segmentObject.transform.position.x, current.segmentObject.transform.position.y - distanceBetweenSegments, current.segmentObject.transform.position.z);
            body.segmentObject.transform.parent = gameObject.transform;
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
                tempNew[y].segmentObject.name = y.ToString();
            }
            segments = tempNew;
        }

        for(int i = 1; i < segments.Length; i++) {
            if (segments[i].segmentObject == null) {
                Split(i);
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
        print(length1 + " " + length2);
        GameObject g = Instantiate(deathapillarPrefab);
        Deathapillar dp = g.GetComponent<Deathapillar>();
        dp.CreateBody(length2, segments);

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

    void Rise() {
        //MOVE BASE TO START
        Vector3 temp = RoomManager.rm.currentRoom.GetRandomNodePosition();
        transform.position = new Vector3(temp.x, depth, temp.z);
        //print(temp);
        //transform.position = temp;
        
        //MOVE HEAD - SLERP
        positionDivingFrom = transform.position; //head's position
        positionToDive = new Vector3(GameManager.gm.p.transform.position.x, depth, GameManager.gm.p.transform.position.z);

        //print("Position Moving From: " + positionDivingFrom);
        //print("Position Moving To: " + positionToDive);

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
        
    }

    IEnumerator RiseEnum(Segment s) {
        while (true) {
            float fracComplete = (Time.time - s.startTime) / journeyTime;
            if (s.segmentObject == null) {
                break;
            }
            s.segmentObject.transform.position = parabola.Move(positionDivingFrom, positionToDive, height, fracComplete);
            yield return null;
            if(fracComplete >= 1) {
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


}
