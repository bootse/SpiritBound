﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GameObject targetUI;

    public class Muskalo : State<PlayerController>
    {
        GameObject chargeEffect;
        float startingInteractTimer = 1.0f;
        float interactTimer = 1.0f;
        public Muskalo()
        {
            HandlerList.Add(new Handler<PlayerController>("Move", move));
            HandlerList.Add(new Handler<PlayerController>("Jump", jump));
            HandlerList.Add(new Handler<PlayerController>("Interact", charge));
            HandlerList.Add(new Handler<PlayerController>("PickUp", pickup));
            HandlerList.Add(new Handler<PlayerController>("PowerZone", powerZone));
        }

        public override void OnEnter(PlayerController owner)
        {
            owner.fox.SetActive(false);
            owner.wolf.SetActive(false);
            owner.muskalo.SetActive(true);
            Physics.gravity = new Vector3(0,-owner.muskaloGravity,0);
			if(owner.inWater) {
				Physics.gravity = new Vector3(0, -owner.waterGravity, 0);
			}
            owner.muskaloTrail.SetActive(true);
            owner.stateMachine.setState("Muskalo");
            Color m = owner.muskaloUI.GetComponent<Image>().color;
            m.a = 1.0f;
            owner.muskaloUI.GetComponent<Image>().color = m;
            owner.foxUI.GetComponent<RectTransform>().localScale = new Vector3(0.8f,0.8f,0.8f);
            owner.wolfUI.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            owner.muskaloUI.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }

        public override void Process(PlayerController owner)
        {
            if (owner.foxPowerLevelUI.GetComponent<Slider>().value < owner.foxSpiritStartPowerLevel) // check if fox power level needs to increase
            {
                owner.foxPowerLevelUI.GetComponent<Slider>().value += Time.deltaTime * 0.5f; //increase fox power level
            }
            else if (owner.foxPowerLevelUI.GetComponent<Slider>().value > owner.foxSpiritStartPowerLevel) //check if fox power level went over starting power level
            {
                owner.foxPowerLevelUI.GetComponent<Slider>().value = owner.foxSpiritStartPowerLevel; // reset value to starting power level
            }

            if (owner.wolfPowerLevelUI.GetComponent<Slider>().value < owner.wolfSpiritStartPowerLevel) // check if wolf power level needs to increase
            {
                owner.wolfPowerLevelUI.GetComponent<Slider>().value += Time.deltaTime * 0.5f; //increase wolf power level
            }
            else if (owner.wolfPowerLevelUI.GetComponent<Slider>().value > owner.wolfSpiritStartPowerLevel)//check if wolf power level went over starting power level
            {
                owner.wolfPowerLevelUI.GetComponent<Slider>().value = owner.wolfSpiritStartPowerLevel;// reset value to starting power level
            }

            if (owner.interacting)
            {
                if (interactTimer > 0)
                {
					interactTimer -= Time.deltaTime;
                }
                else
                {
                    //Destroy(chargeEffect);
                    //owner.interacting = false;
                    //interactTimer = startingInteractTimer;
                    owner.InteractComplete();
                }
            }
        }

        public override void OnExit(PlayerController owner)
        {
            owner.muskaloTrail.SetActive(false);
            Color m = owner.muskaloUI.GetComponent<Image>().color;
            m.a = 0.15f;
            owner.muskaloUI.GetComponent<Image>().color = m;
            if (owner.interacting)
            {
                //Destroy(chargeEffect);
                owner.interacting = false;
                interactTimer = startingInteractTimer;
                owner.InteractComplete();
            }
        }

        void move(PlayerController owner, params object[] args)
        {
            if (owner.duskaloAnimator)
            {
                Vector2 movement = new Vector2((float)args[0], (float)args[1]);
                owner.duskaloAnimator.SetFloat("Speed", movement.magnitude * owner.speed);
                owner.duskaloAnimator.SetBool("Grounded", owner.IsGrounded());
                owner.duskaloAnimator.SetBool("doAction", owner.interacting);
                //owner.duskaloAnimator.SetBool("Refusing", true);
            }
            if (!WallWalk.onWall)
            {
                if (owner.IsGrounded())
                {
                    Vector3 vel = new Vector3((float)args[0] * owner.speed, owner.GetComponent<Rigidbody>().velocity.y, (float)args[1] * owner.speed);
                    if (vel.x != vel.x)
                    {
                        vel.x = 0;
                    }
                    if (vel.y != vel.y)
                    {
                        vel.y = 0;
                    }
                    if (vel.z != vel.z)
                    {
                        vel.z = 0;
                    }
                    owner.GetComponent<Rigidbody>().velocity = vel;
                }
                else
                {
                    owner.AirControlMovement(args);
                }
            }
        }

        void jump(PlayerController owner, params object[] args)
        {
            Vector3 vel = owner.GetComponent<Rigidbody>().velocity;
            vel.y = owner.muskaloJumpPower;
            owner.GetComponent<Rigidbody>().velocity = vel;
        }

        //Lowers head and does a charge/bash foward
        void charge(PlayerController owner, params object[] args)
        {
            //chargeEffect = Instantiate(Resources.Load("ChargeEffect"),owner.transform.position,owner.transform.rotation) as GameObject;
            //chargeEffect.transform.parent = owner.transform;
            //chargeEffect.transform.localPosition = new Vector3(-0.2f,0,1.5f);
            owner.interacting = true;
        }

        void pickup(PlayerController owner, params object[] args)
        {
            if ((string)args[0] == PickUpController.PowerUpType.Score.ToString())
            {
                TimeAndScore.score += 20;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.Coin.ToString())
            {
                TimeAndScore.coins += 1;
                TimeAndScore.score += 10;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.Gem.ToString())
            {
                TimeAndScore.gems += 1;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.WolfPower.ToString())
            {
                if (owner.wolfPowerLevelUI.GetComponent<Slider>().value + 5 > 20)
                    owner.wolfPowerLevelUI.GetComponent<Slider>().value = 20;
                else
                    owner.wolfPowerLevelUI.GetComponent<Slider>().value += 5;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.FoxPower.ToString())
            {
                if (owner.foxPowerLevelUI.GetComponent<Slider>().value + 5 > 20)
                    owner.foxPowerLevelUI.GetComponent<Slider>().value = 20;
                else
                    owner.foxPowerLevelUI.GetComponent<Slider>().value += 5;
            }
        }

        void powerZone(PlayerController owner, params object[] args)
        {
            
        }
    }

    // Sun Spirit is fox
    public class Fox : State<PlayerController>
    {
        GameObject dashEffect;
        float startingInteractTimer = 1.0f;
        float interactTimer = 1.0f;
        public Fox()
        {
            HandlerList.Add(new Handler<PlayerController>("Move", move));
            HandlerList.Add(new Handler<PlayerController>("Jump", jump));
            HandlerList.Add(new Handler<PlayerController>("Interact", dash));
            HandlerList.Add(new Handler<PlayerController>("PickUp", pickup));
            HandlerList.Add(new Handler<PlayerController>("PowerZone", powerZone));
        }

        public override void OnEnter(PlayerController owner)
        {
            owner.fox.SetActive(true);
            owner.wolf.SetActive(false);
            owner.muskalo.SetActive(false);
            Physics.gravity = new Vector3(0, -owner.foxGravity, 0);
			if(owner.inWater) {
				Physics.gravity = new Vector3(0, -owner.waterGravity, 0);
			}
			owner.foxTrail.SetActive(true);
            owner.stateMachine.setState("Fox");
            Color m = owner.foxUI.GetComponent<Image>().color;
            m.a = 1.0f;
            owner.foxUI.GetComponent<Image>().color = m;
            owner.muskaloUI.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            owner.wolfUI.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            owner.foxUI.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }

        public override void Process(PlayerController owner)
        {
            if (!WallWalk.onWall)
            {
                if (owner.foxPowerLevelUI.GetComponent<Slider>().value > 0) // check if fox power level needs to decrease
                {
                    owner.foxPowerLevelUI.GetComponent<Slider>().value -= Time.deltaTime; //decrease fox power level
                }
                else
                {
                    owner.foxPowerLevelUI.GetComponent<Slider>().value = 0;
                    owner.stateMachine.ChangeState(owner.states[0]);
                }
            }
            else
            {
                if (owner.foxPowerLevelUI.GetComponent<Slider>().value - Time.deltaTime > 1) // check if fox power level needs to decrease
                {
                    owner.foxPowerLevelUI.GetComponent<Slider>().value -= Time.deltaTime; //decrease fox power level
                }
                else
                {
                    owner.foxPowerLevelUI.GetComponent<Slider>().value = 1;
                }
            }
            if (owner.interacting)
            {
                if (interactTimer > 0)
                {
                    //Camera.main.GetComponent<ScreenOverlay>().enabled = true;
                    interactTimer -= Time.deltaTime;
                }
                else
                {
                    //Destroy(dashEffect);
                    //Camera.main.GetComponent<ScreenOverlay>().enabled = false;
                    owner.interacting = false;
                    interactTimer = startingInteractTimer;
                    owner.InteractComplete();
                }
            }
        }

        public override void OnExit(PlayerController owner)
        {
            owner.foxTrail.SetActive(false);
            Color m = owner.foxUI.GetComponent<Image>().color;
            m.a = 0.15f;
            owner.foxUI.GetComponent<Image>().color = m;
            if (owner.interacting)
            {
                //Camera.main.GetComponent<ScreenOverlay>().enabled = false;
                //Destroy(dashEffect);
                //owner.interacting = false;
                //interactTimer = startingInteractTimer;
                owner.InteractComplete();
            }
        }

        void move(PlayerController owner, params object[] args)
        {
            if (owner.foxAnimator)
            {
                Vector2 movement = new Vector2((float)args[0], (float)args[1]);
                owner.foxAnimator.SetFloat("Speed", movement.magnitude * owner.speed);
                owner.foxAnimator.SetBool("Grounded", owner.IsGrounded());
                owner.foxAnimator.SetBool("doAction", owner.interacting);
                //owner.foxAnimator.SetBool("Refusing", true);
            }
			if (owner.IsGrounded())
			{
				Vector3 vel = new Vector3((float)args[0] * owner.speed, owner.GetComponent<Rigidbody>().velocity.y, (float)args[1] * owner.speed);
				if(vel.x != vel.x)
				{
					vel.x = 0;
				}
				if(vel.y != vel.y)
				{
					vel.y = 0;
				}
				if(vel.z != vel.z)
				{
					vel.z = 0;
				}
				owner.GetComponent<Rigidbody>().velocity = vel;
			}
			else
            {
                owner.AirControlMovement(args);
            }
        }

        void jump(PlayerController owner, params object[] args)
        {
            Vector3 vel = owner.GetComponent<Rigidbody>().velocity;
            vel.y = owner.foxJumpPower;
            owner.GetComponent<Rigidbody>().velocity = vel;
        }

        //Does a quick dash(speed boost) forward
        void dash(PlayerController owner, params object[] args)
        {
            //if (owner.foxPowerLevelUI.GetComponent<Slider>().value >= 5)
            //{
            //    dashEffect = Instantiate(Resources.Load("FireDash"), owner.transform.position, owner.transform.rotation) as GameObject;
            //    dashEffect.transform.parent = owner.transform;
            //    dashEffect.transform.forward *= -1;
            //    dashEffect.transform.localPosition = new Vector3(0.0f, 1.0f, 2.0f);
            //    owner.foxPowerLevelUI.GetComponent<Slider>().value -= 2;
            //    owner.interacting = true;
            //    if (owner.IsGrounded())
            //        owner.transform.position += new Vector3(0, 0.5f, 0);
            //    owner.rigidbody.AddForce(owner.transform.forward * 15 + owner.fox.transform.up, ForceMode.Impulse);
            //    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Dash"), owner.transform.position);
            //}
            //else
            //{
            //    owner.InteractComplete();
            //}
        }

        void pickup(PlayerController owner, params object[] args)
        {
            if ((string)args[0] == PickUpController.PowerUpType.Score.ToString())
            {
                TimeAndScore.score += 20;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.Coin.ToString())
            {
                TimeAndScore.coins += 1;
                TimeAndScore.score += 10;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.Gem.ToString())
            {
                TimeAndScore.gems += 1;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.WolfPower.ToString())
            {
                if (owner.wolfPowerLevelUI.GetComponent<Slider>().value + 5 > 20)
                    owner.wolfPowerLevelUI.GetComponent<Slider>().value = 20;
                else
                    owner.wolfPowerLevelUI.GetComponent<Slider>().value += 5;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.FoxPower.ToString())
            {
                if (owner.foxPowerLevelUI.GetComponent<Slider>().value + 5 > 20)
                    owner.foxPowerLevelUI.GetComponent<Slider>().value = 20;
                else
                    owner.foxPowerLevelUI.GetComponent<Slider>().value += 5;
            }
        }

        void powerZone(PlayerController owner, params object[] args)
        {
            if ((string)args[0] == PowerZoneController.PowerZoneType.FoxPower.ToString())
            {
                owner.foxPowerLevelUI.GetComponent<Slider>().value += Time.deltaTime;
                if (owner.foxPowerLevelUI.GetComponent<Slider>().value > 20)
                {
                    owner.foxPowerLevelUI.GetComponent<Slider>().value = 20;
                }
            }
        }
    }

    // Moon Spirit is wolf
    public class Wolf : State<PlayerController>
    {
        float startingInteractTimer = 2.25f;
        float interactTimer = 2.25f;
        public Wolf()
        {
            HandlerList.Add(new Handler<PlayerController>("Move", move));
            HandlerList.Add(new Handler<PlayerController>("Jump", jump));
            HandlerList.Add(new Handler<PlayerController>("Interact", precision));
            HandlerList.Add(new Handler<PlayerController>("PickUp", pickup));
            HandlerList.Add(new Handler<PlayerController>("PowerZone", powerZone));
        }

        public override void OnEnter(PlayerController owner)
        {
            owner.fox.SetActive(false);
            owner.wolf.SetActive(true);
            owner.muskalo.SetActive(false);
            Physics.gravity = new Vector3(0, -owner.wolfGravity, 0);
			if(owner.inWater) {
				owner.transform.position = new Vector3(owner.transform.position.x,owner.deadlyWater.transform.position.y,owner.transform.position.z);
			}
            owner.wolfTrail.SetActive(true);
            owner.stateMachine.setState("Wolf");
            Color m = owner.wolfUI.GetComponent<Image>().color;
            m.a = 1.0f;
            owner.wolfUI.GetComponent<Image>().color = m;
            owner.muskaloUI.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            owner.foxUI.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
            owner.wolfUI.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }

        public override void Process(PlayerController owner)
        {
            if (owner.wolfPowerLevelUI.GetComponent<Slider>().value > 0) // check if wolf power level needs to decrease
            {
                owner.wolfPowerLevelUI.GetComponent<Slider>().value -= Time.deltaTime; //decrease wolf power level
            }
            else
            {
                owner.wolfPowerLevelUI.GetComponent<Slider>().value = 0;
                owner.stateMachine.ChangeState(owner.states[0]);
            }

            if (owner.interacting)
            {
                if (interactTimer > 0)
                {
                    //Camera.main.GetComponent<ScreenOverlay>().enabled = true;
                    interactTimer -= Time.deltaTime;
                }
                else
                {
                    //Camera.main.GetComponent<ScreenOverlay>().enabled = false;
                    owner.interacting = false;
                    interactTimer = startingInteractTimer;
                    Time.timeScale = 1f;
                    owner.InteractComplete();
                }
            }
        }

        public override void OnExit(PlayerController owner)
        {
			owner.resetSinker();
            Time.timeScale = 1f;
            owner.wolfTrail.SetActive(false);
            Color m = owner.wolfUI.GetComponent<Image>().color;
            m.a = 0.15f;
            owner.wolfUI.GetComponent<Image>().color = m;
            if (owner.interacting)
            {
                //Camera.main.GetComponent<ScreenOverlay>().enabled = false;
                owner.interacting = false;
                interactTimer = startingInteractTimer;
                Time.timeScale = 1f;
                owner.InteractComplete();
            }
        }

        void move(PlayerController owner, params object[] args)
        {
            if (owner.wolfAnimator)
            {
                Vector2 movement = new Vector2((float)args[0], (float)args[1]);
                owner.wolfAnimator.SetFloat("Speed", movement.magnitude * owner.speed);
                owner.wolfAnimator.SetBool("Grounded", owner.IsGrounded());
                owner.wolfAnimator.SetBool("doAction", owner.interacting);
                //owner.wolfAnimator.SetBool("Refusing", true);
            }
            if (!WallWalk.onWall)
            {
                if (owner.IsGrounded())
                {
                    Vector3 vel = new Vector3((float)args[0] * owner.speed, owner.GetComponent<Rigidbody>().velocity.y, (float)args[1] * owner.speed);
                    if (vel.x != vel.x)
                    {
                        vel.x = 0;
                    }
                    if (vel.y != vel.y)
                    {
                        vel.y = 0;
                    }
                    if (vel.z != vel.z)
                    {
                        vel.z = 0;
                    }
                    owner.GetComponent<Rigidbody>().velocity = vel;
                }
                else
                {
                    owner.AirControlMovement(args);
                }
            }
        }

        void jump(PlayerController owner, params object[] args)
        {
            Vector3 vel = owner.GetComponent<Rigidbody>().velocity;
            vel.x = vel.x * (1f / 1.05f);
            vel.z = vel.z * (1f / 1.05f);
            vel.y = owner.wolfJumpPower;
            owner.GetComponent<Rigidbody>().velocity = vel;
        }

        //Slow down time, player still moves with the same speed
        void precision(PlayerController owner, params object[] args)
        {
            //if (owner.wolfPowerLevelUI.GetComponent<Slider>().value >= 5)
            //{
            //    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/SlowMotion"), owner.transform.position);
            //    owner.wolfPowerLevelUI.GetComponent<Slider>().value -= 2;
            //    owner.interacting = true;
            //    Time.timeScale = 0.5f;
            //}
            //else
            //{
            //    owner.InteractComplete();
            //}
            
        }

        void pickup(PlayerController owner, params object[] args)
        {
            if ((string)args[0] == PickUpController.PowerUpType.Score.ToString())
            {
                TimeAndScore.score += 20;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.Coin.ToString())
            {
                TimeAndScore.coins += 1;
                TimeAndScore.score += 10;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.Gem.ToString())
            {
                TimeAndScore.gems += 1;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.WolfPower.ToString())
            {
                if (owner.wolfPowerLevelUI.GetComponent<Slider>().value + 5 > 20)
                    owner.wolfPowerLevelUI.GetComponent<Slider>().value = 20;
                else
                    owner.wolfPowerLevelUI.GetComponent<Slider>().value += 5;
            }
            else if ((string)args[0] == PickUpController.PowerUpType.FoxPower.ToString())
            {
                if (owner.foxPowerLevelUI.GetComponent<Slider>().value + 5 > 20)
                    owner.foxPowerLevelUI.GetComponent<Slider>().value = 20;
                else
                    owner.foxPowerLevelUI.GetComponent<Slider>().value += 5;
            }
        }

        void powerZone(PlayerController owner, params object[] args)
        {
            if ((string)args[0] == PowerZoneController.PowerZoneType.WolfPower.ToString())
            {
                owner.wolfPowerLevelUI.GetComponent<Slider>().value += Time.deltaTime;
                if (owner.wolfPowerLevelUI.GetComponent<Slider>().value > 20)
                {
                    owner.wolfPowerLevelUI.GetComponent<Slider>().value = 20;
                }
            }
        }
    }

    public class Standby : State<PlayerController>
    {
        public Standby()
        {
            HandlerList.Add(new Handler<PlayerController>("Move", move));
            HandlerList.Add(new Handler<PlayerController>("Jump", jump));
            HandlerList.Add(new Handler<PlayerController>("PickUp", pickup));
        }

        public override void OnEnter(PlayerController owner)
        {
            Physics.gravity = new Vector3(0, -owner.muskaloGravity, 0);
        }

        public override void Process(PlayerController owner)
        {
            
        }

        public override void OnExit(PlayerController owner)
        {

        }

        void move(PlayerController owner, params object[] args)
        {
            if (owner.IsGrounded())
			{
				Vector3 vel = new Vector3((float)args[0] * owner.speed, owner.GetComponent<Rigidbody>().velocity.y, (float)args[1] * owner.speed);
				if(vel.x != vel.x)
				{
					vel.x = 0;
				}
				if(vel.y != vel.y)
				{
					vel.y = 0;
				}
				if(vel.z != vel.z)
				{
					vel.z = 0;
				}
				owner.GetComponent<Rigidbody>().velocity = vel;
			}
            else
            {
                owner.AirControlMovement(args);
            }
        }

        void jump(PlayerController owner, params object[] args)
        {
            Vector3 vel = owner.GetComponent<Rigidbody>().velocity;
            vel.y = owner.muskaloJumpPower;
            owner.GetComponent<Rigidbody>().velocity = vel;
        }

        void pickup(PlayerController owner, params object[] args)
        {

        }

        void powerZone(PlayerController owner, params object[] args)
        {
            
        }
    }

	public class Global : State<PlayerController>
	{
		public Global()
		{

		}

		public override void Process (PlayerController owner)
		{
            if (WallWalk.onWall && owner.hitWall)
            {
                if (owner.RightWall)
                {
                    owner.fox.transform.localEulerAngles = new Vector3(30, 90, 0);
                    owner.wolf.transform.localEulerAngles = new Vector3(30, 90, 0);
                    owner.muskalo.transform.localEulerAngles = new Vector3(30, 90, 0);
                }
                else
                {
                    owner.fox.transform.localEulerAngles = new Vector3(-30, 90, 0);
                    owner.wolf.transform.localEulerAngles = new Vector3(-30, 90, 0);
                    owner.muskalo.transform.localEulerAngles = new Vector3(-30, 90, 0);
                }
            }
            else
            {
                owner.fox.transform.localEulerAngles = new Vector3(0, 90, 0);
                owner.wolf.transform.localEulerAngles = new Vector3(0, 90, 0);
                owner.muskalo.transform.localEulerAngles = new Vector3(0, 90, 0);
            }
			if(owner.inWater)
			{
				Debug.Log("In Water as: "+owner.stateMachine.getState());
				if(owner.stateMachine.getState().Equals("Wolf"))
				{
					if(owner.floorColliders != null)
					{
						owner.floorColliders.GetComponent<Collider>().isTrigger = false;
					}
					Physics.gravity = new Vector3(0, -owner.wolfGravity, 0);
				}
				else
				{
					Debug.Log("hit");
					Physics.gravity = new Vector3(0, -owner.waterGravity, 0);
					if(owner.floorColliders != null)
					{
						owner.floorColliders.GetComponent<Collider>().isTrigger = true;
					}

					owner.sinkTimer -= Time.deltaTime;
					if(owner.sinkTimer < 0)
					{
						owner.Drowning();
					}
				}
			}
			float s = owner.transform.rotation.eulerAngles.y;
			if(s!= s)
			{
				s = 0;
			}
			float xVel = owner.GetComponent<Rigidbody>().velocity.x;
			float zVel = owner.GetComponent<Rigidbody>().velocity.z;
			if(xVel != xVel)
			{
				xVel = 0;
			}
			if(zVel != zVel)
			{
				zVel = 0;
			}

			float e;
			if(Mathf.Abs(xVel) + Mathf.Abs(zVel) < 3)
			{
				e = s;
			}
			else 
			{
				e = Mathf.Rad2Deg*Mathf.Atan2(xVel,zVel);
			}
			float[] vals = owner.getRotationAngles(s,e);
			float valy = (vals[1]-vals[0])/9+vals[0];
			//Debug.Log ("start: "+s+", end: "+e);
			//Debug.Log ("start: "+vals[0]+", end: "+vals[1]);
			//Debug.Log ("Y Rot: "+valy);
			owner.transform.rotation = Quaternion.Euler(owner.transform.rotation.eulerAngles.x,valy,owner.transform.rotation.eulerAngles.z);
		}
	}

    public State<PlayerController>[] states = new State<PlayerController>[] {	new Muskalo(),
																				new Wolf(),
                                                                                new Fox(),
                                                                                new Standby(),
																				new Global()};
    public StateMachine<PlayerController> stateMachine = new StateMachine<PlayerController>();

    public GameObject muskaloUI;
    public GameObject wolfUI;
    public GameObject foxUI;
    public GameObject foxPowerLevelUI;
    public GameObject wolfPowerLevelUI;
	public GameObject floorColliders;

    public GameObject fox;
    public GameObject wolf;
    public GameObject muskalo;

	public GameObject followingCamera;
	private float sinkTimer = 0;
	public float resetSinkTime = 1f;
	public bool inWater { get; private set; }
	private killPlayer deadlyWater;
	//private float camDirection = 0;
	private float offset = 30;
	private float setRotation = 0;
	public bool useRotation = false;
	public void newRotation(float r) {
		setRotation = r;
	}
	public float getRotation() {
		return setRotation;
	}

    float foxSpiritStartPowerLevel = 20;
    float wolfSpiritStartPowerLevel = 20;

    public GameObject foxTrail;
    public GameObject wolfTrail;
    public GameObject muskaloTrail;

    public bool grounded; // Determine if the player is on the ground
    public bool hitWall = false;
    float distToGround; // Distance from player collider to ground

    public bool interacting = false;

    [Tooltip("How fast the player will move")]
    public float speed = 1; //How fast the player will move

    [Tooltip("The force of gravity on the muskalo")]
    public float muskaloGravity = 15; //force of gravity on the muskalo

	[Tooltip("The force of gravity in water")]
	public float waterGravity = 8; //force of gravity in the water

    [Tooltip("The force of gravity on the fox")]
    public float foxGravity = 10; //force of gravity on the fox

    [Tooltip("The force of gravity on the wolf")]
    public float wolfGravity = 40; //force of gravity on the wolf

    [Tooltip("The jump power of the muskalo")]
    public float muskaloJumpPower = 5; //jump power of the muskalo

    [Tooltip("The jump power of the fox")]
    public float foxJumpPower = 5; //jump power of the fox

    [Tooltip("The jump power of the wolf")]
    public float wolfJumpPower = 20; //jump power of the wolf

    public Animator duskaloAnimator;
    public Animator foxAnimator;
    public Animator wolfAnimator;

    Vector3 wolfStartUp;
    Vector3 foxStartUp;
    Vector3 muskaloStartUp;

    bool RightWall = false;

    //Vector3 startGravity = new Vector3(0,-10f,0); //Variable to tell what the starting gravity was

    // Use this for initialization
    void Start()
    {
		#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
		wolfJumpPower = wolfJumpPower * 1.1f;
		foxJumpPower = foxJumpPower * 1.1f;
		muskaloJumpPower = muskaloJumpPower * 1.1f;
		#endif
        foxPowerLevelUI.GetComponent<Slider>().value = 15;
        wolfPowerLevelUI.GetComponent<Slider>().value = 15;
        wolfStartUp = wolf.transform.up;
        foxStartUp = fox.transform.up;
        muskaloStartUp = muskalo.transform.up;
        distToGround = GetComponent<Collider>().bounds.extents.y; // set distance from player collider to ground
        stateMachine.Configure(this, states[0]);
		stateMachine.ChangeGlobalState(states[4]);
		resetSinker();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("is grounded " + IsGrounded());
        Debug.Log("is in water " + inWater);
        Debug.Log("on the wall " + WallWalk.onWall);
        if (IsGrounded() && !WallWalk.onWall && (!inWater || stateMachine.getState().Equals("Wolf")))
        {
            Debug.Log("can jump");
        }
        else
        {
            Debug.Log("player cant jump");
        }
        stateMachine.Update();
    }


    /***************************
     ******** New Calls ********
     ***************************/

    public void ChangeToWolf()
    {
        if (!WallWalk.onWall && GetComponent<InputController>().receiveInput)
            stateMachine.ChangeState(states[1]);
    }
    public void ChangeToFox()
    {
        if(GetComponent<InputController>().receiveInput)
            stateMachine.ChangeState(states[2]);
    }
    public void ChangeToMuskalo()
    {
        if (!WallWalk.onWall && GetComponent<InputController>().receiveInput)
            stateMachine.ChangeState(states[0]);
    }

    void Move(Vector2 direction)
    {
		if(!stateMachine.getState().Equals("Wolf") && inWater) {
			direction = direction/3;
		}
		if(direction.y > 2.5f)
		{
			float y = direction.y;
			direction = direction*1f/2f;
			direction.y = y;
		}
		else if(direction.y < 0)
		{
			direction = direction*1f/4f;
			direction.y = 0;
		}
		else
		{
			float y = direction.y;
			direction = direction*1f/4f;
			direction.y = y;
		}
		float rotation = followingCamera.transform.rotation.eulerAngles.y;

		stateMachine.messageReciever("Move",new object[] {Mathf.Cos(rotation*Mathf.Deg2Rad)*direction.x + 
			Mathf.Sin(rotation*Mathf.Deg2Rad)*direction.y,Mathf.Cos(rotation*Mathf.Deg2Rad)*direction.y - 
			Mathf.Sin(rotation*Mathf.Deg2Rad)*direction.x});// straight
    }

    void Interact()
    {
        interacting = true;
        stateMachine.messageReciever("Interact", null);
    }

    void PickUp(string powerUpType)
    {
        stateMachine.messageReciever("PickUp", powerUpType);
    }

    void PowerZone(string powerZoneType)
    {
        stateMachine.messageReciever("PowerZone", powerZoneType);
    }

    bool IsGrounded()
    {
        Debug.DrawLine(transform.position + 1.25f * transform.forward, transform.position + 1.25f * transform.forward+(distToGround + 0.1f)*-transform.up, Color.red);
        Debug.DrawLine(transform.position + 1.25f * transform.forward - 0.4f * transform.right, transform.position + 1.25f * transform.forward - 0.4f * transform.right + (distToGround + 0.1f) * -transform.up, Color.red);
        Debug.DrawLine(transform.position + 1.25f * transform.forward + 0.4f * transform.right, transform.position + 1.25f * transform.forward + 0.4f * transform.right + (distToGround + 0.1f) * -transform.up, Color.red);
        if (Physics.Raycast(transform.position + 1.25f * transform.forward, -transform.up, distToGround + 0.15f))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + 1.25f * transform.forward + 0.4f*transform.right, -transform.up, distToGround + 0.15f))
        {
            return true;
        }
        else if (Physics.Raycast(transform.position + 1.25f * transform.forward - 0.4f*transform.right, -transform.up, distToGround + 0.15f))
        {
            return true;
        }
        else
        {
            return false;
        }
        //Debug.DrawLine(transform.position + 1.25f*transform.forward, transform.position+1.25f*transform.forward+-distToGround*transform.up,Color.red);
		//return ; // Raycast to determine if player is on the ground and they can jump
    }

    /* If the player is grounded they will jump*/
    void Jump()
    {
        if (IsGrounded() && !WallWalk.onWall && (!inWater || stateMachine.getState().Equals("Wolf")))
        {
            stateMachine.messageReciever("Jump",null);
        }
    }

    void AirControlMovement(params object[] args)
    {
        float airControl = 0.5f;
        float airSpeed = speed;
        Vector3 airMove = new Vector3((float)args[0] * airSpeed * (1 / Time.timeScale), GetComponent<Rigidbody>().velocity.y, (float)args[1] * airSpeed) * (1 / Time.timeScale);
        if(airMove.x != airMove.x)
		{
			airMove.x = 0;
		}
		if(airMove.y != airMove.y)
		{
			airMove.y = 0;
		}
		if(airMove.z != airMove.z)
		{
			airMove.z = 0;
		}
		Vector3 vel = Vector3.Lerp(GetComponent<Rigidbody>().velocity, airMove, Time.deltaTime * airControl);
		if(vel.x != vel.x || float.IsInfinity(vel.x) )
		{
			vel.x = 0;
		}
		if(vel.y != vel.y || float.IsInfinity(vel.y) )
		{
			vel.y = 0;
		}
		if(vel.z != vel.z || float.IsInfinity(vel.z) )
		{
			vel.z = 0;
		}
		GetComponent<Rigidbody>().velocity = vel;
    }

	public void InteractComplete()
	{
        this.SendMessage("interactFinished", SendMessageOptions.DontRequireReceiver);
	}

    void SwitchToStandby()
    {
        stateMachine.ChangeState(states[3]);
    }

	public float[] getRotationAngles(float start, float end) {
		if(Mathf.Abs(start-end) <= 180) {
			return new float[] {start,end};
		}
		if(start < 180) {
			if(end < 0) {
				return new float[] {start,end+360};
			}
			return new float[] {start,end-360};
		}
		if(end > 360) {
			return new float[] {start,end-360};
		}
		return new float[] {start,end+360};
	}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Angled_Walls")
        {
            //Debug.Log("Got on the angled walls");
            hitWall = true;
            ContactPoint whereItHit = other.contacts[0];
            //Physics.gravity = -whereItHit.normal;
            Debug.Log("on collision enter");
            if (transform.InverseTransformDirection(transform.position-whereItHit.point).x > 0)
            {
                RightWall = true;
            }
            else
            {
                RightWall = false;
            }
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Angled_Walls")
        {
            Debug.Log("on collision exit");
            if (stateMachine.getState().Equals("Muskalo"))
            {
                Physics.gravity = new Vector3(0, -muskaloGravity, 0);
            }
            if (stateMachine.getState().Equals("Wolf"))
            {
                Physics.gravity = new Vector3(0, -wolfGravity, 0);
            }
            if (stateMachine.getState().Equals("Fox"))
            {
                Physics.gravity = new Vector3(0, -foxGravity, 0);
            }
        }
    }

	void OnTriggerEnter(Collider other) {
		string layer = LayerMask.LayerToName(other.gameObject.layer);
		string tag = other.tag;
		if(layer.Equals("Water")) {
			inWater = true;
			deadlyWater = other.GetComponent<killPlayer>();
			resetSinker();
		}
		if(stateMachine.getState().Equals("Muskalo")) {
			if(tag.Equals("Breakable")) 
			{
				stateMachine.messageReciever("Interact", null);
				duskaloAnimator.SetTrigger("Bash");
			}
		}
		if(stateMachine.getState().Equals("Wolf")) {

		}
		if(stateMachine.getState().Equals("Fox")) {

		}
	}

	void OnTriggerStay(Collider other) {
		string tag = other.tag;
		if(stateMachine.getState().Equals("Muskalo")) {
			if(tag.Equals("Breakable")) {
				stateMachine.messageReciever("Interact", null);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		string layer = LayerMask.LayerToName(other.gameObject.layer);
		if(layer.Equals("Water")) {
			inWater = false;
			if(floorColliders != null)
			{
				floorColliders.GetComponent<Collider>().isTrigger = false;
			}
			if(stateMachine.getState().Equals("Muskalo")) {
				Physics.gravity = new Vector3(0, -muskaloGravity, 0);
			}
			if(stateMachine.getState().Equals("Wolf")) {
				Physics.gravity = new Vector3(0, -wolfGravity, 0);
			}
			if(stateMachine.getState().Equals("Fox")) {
				Physics.gravity = new Vector3(0, -foxGravity, 0);
			}
		}
	}

	public void Drowning() {
		deadlyWater.playerDied(this.GetComponent<Collider>());
	}

	public void playerDied(Vector3 cam, Quaternion rot) {
		ChangeToMuskalo();
		followingCamera.GetComponent<CameraController>().setPosition(cam, rot);
		GetComponent<InputController>().ySpeed = 0;
		GameObject con = GameObject.Find("MobileControls");
		if(con != null)
		{
			con.GetComponent<MobileControls>().ySpeed = 0;
		}
		GetComponent<InputController>().receiveInput = true;
	}

	public void resetSinker() {
		sinkTimer = resetSinkTime;
	}
}
