var CloudColor : Color;
var CloudHighlight : Color;
var offset = 0.2;
var particleHeight : float;
var CloudAlt : float;
var CloudHeight =60;
function LateUpdate () {
CloudAlt = transform.position.y-30;

var particles = GetComponent.<ParticleEmitter>().particles;
for (var i=0; i<particles.Length; i++) {


particleHeight= (((particles[i].position.y - CloudAlt)/CloudHeight)-offset);
particles[i].color = Color.Lerp (CloudColor,CloudHighlight,particleHeight);



}


GetComponent.<ParticleEmitter>().particles = particles;
}