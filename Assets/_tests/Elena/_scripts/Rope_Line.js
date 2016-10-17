//============================
//==	Physics Based 3D Rope ==
//==	File: Rope_Tube.js	==
//==	By: Jacob Fletcher	==
//==	Use and alter Freely	==
//============================
//To see other things I have created, visit me at www.reverieinterative.com
//How To Use:
// ( BASIC )
// 1. Simply add this script to the object you want a rope teathered to
// 3. Assign the other end of the rope as the "Target" object in this script
// 4. Play and enjoy!
// (About Character Joints)
// Sometimes your rope needs to be very limp and by that I mean NO SPRINGY EFFECT.
// In order to do this, you must loosen it up using the swingAxis and twist limits.
// For example, On my joints in my drawing app, I set the swingAxis to (0,0,1) sense
// the only axis I want to swing is the Z axis (facing the camera) and the other settings to around -100 or 100.
/*var target : Transform;
var material : Material;
var ropeWidth = 0.5;
var resolution = 0.5;
var ropeDrag = 0.1;
var ropeMass = 0.5;
var radialSegments = 6;
var startRestrained = true;
var endRestrained = false;
var useMeshCollision = false;
// Private Variables (Only change if you know what your doing)
private var segmentPos : Vector3[];
private var joints : GameObject[];
private var tubeRenderer : GameObject;
private var line : TubeRenderer;
private var segments = 4;
private var rope = false;
//Joint Settings
var swingAxis = Vector3(0,1,0);
var lowTwistLimit = 0.0;
var highTwistLimit = 0.0;
var swing1Limit	= 20.0;
// Require a Rigidbody
@script RequireComponent(Rigidbody)
 
function OnDrawGizmos() {
    if(target) {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine (transform.position, target.position);
        Gizmos.DrawWireSphere ((transform.position+target.position)/2,ropeWidth);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, ropeWidth);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (target.position, ropeWidth);
    } else {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, ropeWidth);	
    }
}
 
function Awake()
{
    if(target) {
        BuildRope();
    } else {
        Debug.LogError("You must have a gameobject attached to target: " + this.name,this);	
    }
}
 
function LateUpdate()
{
    if(target) {
        // Does rope exist? If so, update its position
        if(rope) {
            line.SetPoints(segmentPos, ropeWidth, Color.white);
            line.enabled = true;
            segmentPos[0] = transform.position;
            for(var s:int=1;s<segments;s++)
            {
			 segmentPos[s] = joints[s].transform.position;
            }
    }
}
}
 
function BuildRope()
{
    tubeRenderer = new GameObject("TubeRenderer_" + gameObject.name);
    line = tubeRenderer.AddComponent(TubeRenderer);
    line.useMeshCollision = useMeshCollision;
    // Find the amount of segments based on the distance and resolution
    // Example: [resolution of 1.0 = 1 joint per unit of distance]
    segments = Vector3.Distance(transform.position,target.position)*resolution;
    if(material) {
        material.SetTextureScale("_MainTex",Vector2(1,segments+2));
        if(material.GetTexture("_BumpMap"))
            material.SetTextureScale("_BumpMap",Vector2(1,segments+2));
    }
    line.vertices = new TubeVertex[segments];
    line.crossSegments = radialSegments;
    line.material = material;
    segmentPos = new Vector3[segments];
    joints = new GameObject[segments];
    segmentPos[0] = transform.position;
    segmentPos[segments-1] = target.position;
    // Find the distance between each segment
    var segs = segments-1;
    var seperation = ((target.position - transform.position)/segs);
    for(var s:int=0;s < segments;s++)
    {
        // Find the each segments position using the slope from above
		var vector : Vector3 = (seperation*s) + transform.position;	
		segmentPos[s] = vector;
        //Add Physics to the segments
		AddJointPhysics(s);
    }
// Attach the joints to the target object and parent it to this object
var end : CharacterJoint = target.gameObject.AddComponent.<CharacterJoint>();
end.connectedBody = joints[joints.length-1].transform.GetComponent.<Rigidbody>();
end.swingAxis = swingAxis;
end.lowTwistLimit.limit = lowTwistLimit;
end.highTwistLimit.limit = highTwistLimit;
end.swing1Limit.limit	= swing1Limit;
target.parent = transform;
if(endRestrained)
{
    end.GetComponent.<Rigidbody>().isKinematic = true;
}
if(startRestrained)
{
    transform.GetComponent.<Rigidbody>().isKinematic = true;
}
// Rope = true, The rope now exists in the scene!
rope = true;
}
 
function AddJointPhysics(n : int)
    {
        joints[n] = new GameObject("Joint_" + n);
        joints[n].transform.parent = transform;
        var rigid : Rigidbody = joints[n].AddComponent.<Rigidbody>();
        if(!useMeshCollision) {
            var col : SphereCollider = joints[n].AddComponent.<SphereCollider>();
            col.radius = ropeWidth;
        }
        var ph : CharacterJoint = joints[n].AddComponent.<CharacterJoint>();
        ph.swingAxis = swingAxis;
        ph.lowTwistLimit.limit = lowTwistLimit;
        ph.highTwistLimit.limit = highTwistLimit;
        ph.swing1Limit.limit	= swing1Limit;
        //ph.breakForce = ropeBreakForce; <--------------- TODO
        joints[n].transform.position = segmentPos[n];
        rigid.drag = ropeDrag;
        rigid.mass = ropeMass;
        if(n==0){			ph.connectedBody = transform.GetComponent.<Rigidbody>();
        } else
        {
            ph.connectedBody = joints[n-1].GetComponent.<Rigidbody>();	
        }
    }*/