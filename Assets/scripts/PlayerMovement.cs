using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{

    public float thrust = 0.1f;
	[Range(0.0f, 1.0f)]public float wallfriction = 0.75f;
	[Range(0.0f, 1.0f)]public float boxSlowDown = 0.5f;
    public Rigidbody2D rb;
    public Vector3 mousePos;
    public Vector2 force;
	Tilemap[] tnt_breakables = new Tilemap[3];
	bool pressed;
    // Start is called before the first frame update
    void Start()
    {
		tnt_breakables[0] = GameObject.Find("box").GetComponent<Tilemap>();
		tnt_breakables[1] = GameObject.Find("tnt_breakable").GetComponent<Tilemap>();
		tnt_breakables[2] = GameObject.Find("tnt").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		float Horizontaldist = 0;
		float Verticaldist = 0;
		mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if(Input.GetMouseButton(0)){
			pressed = true;
		}
		if(pressed == true && Input.GetMouseButton(0) == false){
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Horizontaldist = mousePos.x - transform.position.x;
			Verticaldist = mousePos.y - transform.position.y;
			force = new Vector2(-Horizontaldist, -Verticaldist);
			rb.velocity = Vector2.zero;
			rb.AddRelativeForce(force * thrust);
			pressed = false;
		}
    }
	void OnCollisionEnter2D(Collision2D collision){
		if(collision.gameObject.CompareTag("wall") || collision.gameObject.CompareTag("turnable") || collision.gameObject.CompareTag("tnt_breakable")){
			rb.velocity = rb.velocity*wallfriction;
		} else if(collision.gameObject.CompareTag("box")){
			Vector3 hitPosition = Vector3.zero;
			foreach(ContactPoint2D hit in collision.contacts){
				hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
				hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
				Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
				tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
			}
			rb.velocity = -rb.velocity*boxSlowDown;
		} else if(collision.gameObject.CompareTag("tnt")){
			Vector3 hitPosition = Vector3.zero;
			foreach(ContactPoint2D hit in collision.contacts){
				hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
				hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
				OnTntDestroyed(hitPosition.x, hitPosition.y, tnt_breakables, GameObject.Find("tnt").GetComponent<Tilemap>());
			}
		}
	}

	void OnTntDestroyed(float xPos, float yPos, Tilemap[] tnt_breakables, Tilemap tnt_tilemap){
		for(int i = 0; i < tnt_breakables.Length; i++){
			for(int x = 0; x < 3; x++){
				for(int y = 0; y < 3; y++){
					Vector2 pos = new Vector2(xPos + x - 1f, yPos + y - 1f);
					if(tnt_breakables[i] == tnt_tilemap && tnt_breakables[i].WorldToCell(pos) != null && x != 1 && y != 1){
						Debug.Log("triggered");
						tnt_breakables[i].SetTile(tnt_breakables[i].WorldToCell(pos), null);
						OnTntDestroyed(xPos + x - 1f, yPos + y - 1f, tnt_breakables, tnt_tilemap);
					} else {
						tnt_breakables[i].SetTile(tnt_breakables[i].WorldToCell(pos), null);
					}
				}
			}
		}
	}
}
