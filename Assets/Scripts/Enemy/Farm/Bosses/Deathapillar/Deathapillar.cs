using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathapillar : MonoBehaviour {

    public List<Segment> segments;
    public Segment headSegment;
    [Header("Deathapillar Settings")]
    public GameObject segmentPrefab;
    public float distanceBetweenSegments = 50f;
    public float speed = 5f;
    public float diveMagnitude = 1f;
    public float depth = -30f;
    public State state = State.IDLE;
    public float idleTime = 3f;
    private float startIdleTime;

    public Vector3 positionDivingFrom;
    private Vector3 positionToDive;
    private Vector3 centerPoint;
    private Vector3 startRelCenter;
    private Vector3 endRelCenter;

    public enum State {
        IDLE,
        DIVING
    }

    [System.Serializable]
    public class Segment : Enemy {
        public bool isHead = false;
        public Segment before;
        public Segment after;
        public GameObject segmentObject;

        public Coroutine diveCoroutine;
        public float startTime;
    }

    private void Start() {
        CreateBody(5);
        //positionToDive = new Vector3(GameManager.gm.p.transform.position.x, depth, GameManager.gm.p.transform.position.z);
        positionToDive = GameManager.gm.p.transform.position;
        startIdleTime = Time.time;
    }

    // Update is called once per frame
    private void Update() {

        switch (state) {
            case State.IDLE:
                if(Time.time >= startIdleTime + idleTime) {
                    GetCenterOfDirection(Vector3.up);
                    Rise();
                    state = State.DIVING;
                }
                break;
            case State.DIVING:
                if (!CheckIfDiving()) {
                    ResetSegments();
                    state = State.IDLE;
                }
                break;
        }

        //Rise();
    }




    void CreateBody(int i) {
        //CREATE SEGMENTS OF LENGTH i

        //CREATE HEAD
        Segment head = new Segment {
            segmentObject = Instantiate(segmentPrefab),
            isHead = true
        };
        segments.Add(head);
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
            segments.Add(body);
            current.after = body;
            body.segmentObject.transform.position = new Vector3(current.segmentObject.transform.position.x, current.segmentObject.transform.position.y - distanceBetweenSegments, current.segmentObject.transform.position.z);
            body.segmentObject.transform.parent = gameObject.transform;
            current = body;
        }
    }

    void Split() {
        //SPLIT BODY INTO TWO SEPERATE MONSTERS
    }

    void PlaySound() {

    }

    void Rise() {
        //MOVE BASE TO START
        Vector3 temp = RoomManager.rm.currentRoom.GetRandomNodePosition();
        //transform.position = new Vector3(temp.x, depth, temp.z);
        print(temp);
        transform.position = temp;
        
        //MOVE HEAD - SLERP
        positionDivingFrom = transform.position; //head's position
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
            float fracComplete = (Time.time - s.startTime) / speed;
            s.segmentObject.transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete);
            s.segmentObject.transform.position += centerPoint;
            yield return null;
            if(fracComplete >= 1) {
                break;
            }
        }
        s.diveCoroutine = null;
    }

    IEnumerator MoveToHeadPosition(Segment s, int i) {
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

    void GetCenterOfDirection(Vector3 direction) {
        centerPoint = (positionDivingFrom + positionToDive) * .5f;
        centerPoint -= (direction * diveMagnitude);
        startRelCenter = positionDivingFrom - centerPoint;
        endRelCenter = positionToDive - centerPoint;
    }

    bool CheckIfDiving() {
        foreach(Segment s in segments) {
            if(s.diveCoroutine != null) {
                return true;
            }
        }
        return false;
    }


}
