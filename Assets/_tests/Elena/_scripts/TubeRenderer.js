//TubeRenderer.js
 
//This script is created by Ray Nothnagel of Last Bastion Games. It is
//free for use and available on the Unify Wiki.
 
//For other components I've created, see:
//http://lastbastiongames.com/middleware/
 
//(C) 2008 Last Bastion Games
 
//--------------------------------------------------------------
 
//EDIT: MODIFIED BY JACOB FLETCHER FOR USE WITH THE ROPE SCRIPT
//http://www.reverieinteractive.com
 
 
/*class TubeVertex {
    var point : Vector3 = Vector3.zero;
	var radius : float = 1.0;
	var color : Color = Color.white;
 
	function TubeVertex(pt : Vector3, r : float, c : Color) {
	    point=pt;
	    radius=r;
	    color=c;
	}
	}
 
        var vertices : TubeVertex[];
        var material : Material;
        var crossSegments : int = 3;
        var flatAtDistance : float=-1;
        var movePixelsForRebuild = 6;
        var maxRebuildTime = 0.1;
        var useMeshCollision = false;
 
private var lastCameraPosition1 : Vector3;
private var lastCameraPosition2 : Vector3;
private var crossPoints : Vector3[];
private var lastCrossSegments : int;
private var lastRebuildTime = 0.00;
private var mesh : Mesh;
private var colliderExists = false;
private var usingBumpmap = false;
 
        function Reset() {
            vertices = [TubeVertex(Vector3.zero, 1.0, Color.white), TubeVertex(Vector3(1,0,0), 1.0, Color.white)];
        }
 
        function Start() {
            Reset();
            mesh = new Mesh();
            gameObject.AddComponent(MeshFilter);
            var mr : MeshRenderer = gameObject.AddComponent(MeshRenderer);
            mr.material = material;
            if(material) {
                if(material.GetTexture("_BumpMap")) usingBumpmap = true;
            }
        }
 
        function LateUpdate () {
            if (!vertices || vertices.length <= 1) {
                GetComponent.<Renderer>().enabled=false;
                return;
            }
 
            GetComponent.<Renderer>().enabled=true;
            if (crossSegments != lastCrossSegments) {
                crossPoints = new Vector3[crossSegments];
                var theta : float = 2.0*Mathf.PI/crossSegments;
                for (var c:int=0;c<crossSegments;c++) {
                    crossPoints[c] = Vector3(Mathf.Cos(theta*c), Mathf.Sin(theta*c), 0);
                }
            lastCrossSegments = crossSegments;
        }
 
        var meshVertices : Vector3[] = new Vector3[vertices.length*crossSegments];
        var uvs : Vector2[] = new Vector2[vertices.length*crossSegments];
        var colors : Color[] = new Color[vertices.length*crossSegments];
        var tris : int[] = new int[vertices.length*crossSegments*6];
        var lastVertices : int[] = new int[crossSegments];
        var theseVertices : int[] = new int[crossSegments];
        var rotation : Quaternion;
 
        for (var p:int=0;p<vertices.length;p++) {
            if(p<vertices.length-1)
                rotation = Quaternion.FromToRotation(Vector3.forward,vertices[p+1].point-vertices[p].point);
 
            for (c=0;c<crossSegments;c++) {
                var vertexIndex : int = p*crossSegments+c;
                meshVertices[vertexIndex] = vertices[p].point + rotation * crossPoints[c] * vertices[p].radius;
                uvs[vertexIndex] = Vector2((0.0+c)/crossSegments,(0.0+p)/vertices.length);
                colors[vertexIndex] = vertices[p].color;
 
                lastVertices[c]=theseVertices[c];
                theseVertices[c] = p*crossSegments+c;
        }
 
    //make triangles
    if (p>0) {
        for (c=0;c<crossSegments;c++) {
            var start : int= (p*crossSegments+c)*6;
            tris[start] = lastVertices[c];
            tris[start+1] = lastVertices[(c+1)%crossSegments];
            tris[start+2] = theseVertices[c];
            tris[start+3] = tris[start+2];
            tris[start+4] = tris[start+1];
            tris[start+5] = theseVertices[(c+1)%crossSegments];
        }
    }
    }
 
    //Clear mesh for new build  (jf)	
    mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        if(usingBumpmap)
            mesh.tangents = CalculateTangents(meshVertices);
        mesh.uv = uvs;
 
        if(useMeshCollision)
            if(colliderExists) {
                gameObject.GetComponent(MeshCollider).sharedMesh = mesh;
            } else {
                gameObject.AddComponent(MeshCollider);
                colliderExists = true;
            }
        GetComponent(MeshFilter).mesh = mesh;
    }
 
 
 
    function CalculateTangents(verts : Vector3[])
    {
        var tangents : Vector4[] = new Vector4[verts.length];
 
        for(var i:int=0;i<tangents.length;i++)
        {
            var vertex1 = i > 0 ? verts[i-1] : verts[i];
              var vertex2 = i < tangents.length - 1 ? verts[i+1] : verts[i];
              var tan = (vertex1 - vertex2).normalized;
              tangents[i] = Vector4( tan.x, tan.y, tan.z, 1.0 );
        }
    return tangents;	
    }
 
 
 
//sets all the points to points of a Vector3 array, as well as capping the ends.
function SetPoints(points : Vector3[], radius : float, col : Color) {
    if (points.length < 2) return;
    vertices = new TubeVertex[points.length+2];
 
    var v0offset : Vector3 = (points[0]-points[1])*0.01;
    vertices[0] = TubeVertex(v0offset+points[0], 0.0, col);
    var v1offset : Vector3 = (points[points.length-1] - points[points.length-2])*0.01;
    vertices[vertices.length-1] = TubeVertex(v1offset+points[points.length-1], 0.0, col);
 
    for (var p:int=0;p<points.length;p++) {
		vertices[p+1] = TubeVertex(points[p], radius, col);
    }
}*/