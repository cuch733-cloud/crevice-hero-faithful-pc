using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
	private enum STATE
	{
		ST_PLAY,
		ST_ZOMBIE,
		ST_DEAD
	}

	public GameObject skillEffectObject;

	public GameObject dustObject;

	public GameObject safeObject;

	public tk2dTextMesh skillRemainText;

	public GameObject flyTargetObject;

	private float maxWalkSpeed = 7f;

	private float maxFlySpeed = 6f;

	private float jumpForce = 580f;

	private float GRAVITY_SCALE = 4f;

	private float skillFlySpeed = 10f;

	private float skillTeleportSpeed = 25f;

	private float speedRate = 1f;

	private float jumpDuration;

	private float moveVel;

	private bool isJump;

	private float h;

	private bool isGrounded;

	private bool isHeadHit;

	private bool isTrans;

	private float maxSpeed;

	private Vector3 groundLeft;

	private Vector3 groundRight;

	private Vector3 groundCenter;

	private Vector3 groundCenter2;

	private Vector3 headCheck;

	private Vector3 deadCheck;

	private tk2dSprite playerSprite;

	private tk2dSpriteAnimator playerAnimator;

	private tk2dSpriteAnimator playerDustAnimator;

	private tk2dSpriteAnimator playerSafeAnimator;

	private tk2dSpriteAnimator skillEffectAnimator;

	private tk2dSpriteAnimation skillEffectAnimation;

	private float lastX;

	private tk2dSpriteAnimationClip sitClip;

	private tk2dSpriteAnimationClip moveClip;

	private tk2dSpriteAnimationClip jumpClip;

	private tk2dSpriteAnimationClip stayClip;

	private static Vector3 safeOffset = new Vector3(0f, 1.2f, 0f);

	private STATE state;

	private int activeSkillID;

	private int currentID = -1;

	private PlayerData data;

	public static Hero Instance = null;

	private GameObject skillBG;

	private GameObject skillBGPrefab;

	private bool isLoaded;

	private bool isSkillClip;

	private bool isTeleport;

	private bool isTeleportMoving;

	private int teleportCount;

	private Transform ballTf;

	private Transform missileTf1;

	private Transform missileTf2;

	private float ballVelX;

	private float balldistX;

	private float balldistY;

	[HideInInspector]
	public Rock flyTargetRock;

	private Vector3 flyTargetVector;

	private float flyVelX;

	private float flyVelY;

	private bool isHammering;

	public bool isBallShoot;

	public bool isBallSkill;

	private bool isMissile;

	private bool isIgnoreUpRock;

	private void Start()
	{
	}

	private void Awake()
	{
		playerDustAnimator = dustObject.GetComponent<tk2dSpriteAnimator>();
		playerSafeAnimator = safeObject.GetComponent<tk2dSpriteAnimator>();
		dustObject.SetActive(value: false);
		safeObject.SetActive(value: false);
		skillEffectAnimator = skillEffectObject.GetComponent<tk2dSpriteAnimator>();
		skillEffectObject.SetActive(value: false);
		skillEffectAnimation = skillEffectAnimator.Library;
		Instance = this;
	}

	public float GetX()
	{
		return lastX;
	}

	private void UpdateDataByID()
	{
	}

	private void UpdateByID()
	{
		BoxCollider2D component = base.gameObject.GetComponent<BoxCollider2D>();
		component.size = new Vector2(data.width, data.height);
		float num = 0f;
		if (data.GetSkillID() == 13)
		{
			num = 0.04f;
		}
		component.center = new Vector2(0f, data.height / 2f - num);
		groundLeft = new Vector3((0f - component.size.x) / 2f + 0.01f, -0.04f - num, 0f);
		groundRight = new Vector3(component.size.x / 2f - 0.01f, -0.04f - num, 0f);
		groundCenter = new Vector3(0f, -0.04f - num, 0f);
		groundCenter2 = new Vector3(0f, -4f - num, 0f);
		headCheck = new Vector3(0f, component.size.y - 0.08f, 0f);
		deadCheck = new Vector3(0f, component.size.y / 2f - 0.2f, 0f);
		playerSprite = GetComponent<tk2dSprite>();
		playerAnimator = GetComponent<tk2dSpriteAnimator>();
		playerSprite.scale = new Vector3(1f, 1f, 1f);
		playerSprite.SortingOrder = 0;
		playerSprite.color = new Color(1f, 1f, 1f, 1f);
		string path = $"char{data.animId}Animation";
		playerAnimator.Library = Resources.Load(path, typeof(tk2dSpriteAnimation)) as tk2dSpriteAnimation;
		playerAnimator.AnimationCompleted = null;
		base.gameObject.SetActive(value: true);
		moveClip = playerAnimator.GetClipByName("moveClip");
		stayClip = playerAnimator.GetClipByName("stayClip");
		jumpClip = playerAnimator.GetClipByName("jumpClip");
		if (jumpClip == null)
		{
			jumpClip = moveClip;
		}
		sitClip = playerAnimator.GetClipByName("sitClip");
		if (sitClip == null)
		{
			sitClip = stayClip;
		}
		playerAnimator.Play(stayClip);
		isLoaded = true;
		isSkillClip = false;
		isTeleport = false;
		isTeleportMoving = false;
	}

	private void LoadPlayer(bool isLoad)
	{
		Debug.Log("LoadPlayer " + isLoad);
		switch (data.GetSkillID())
		{
		case 1:
			if (isLoad)
			{
				skillBGPrefab = Resources.Load("skill0BG") as GameObject;
				skillBG = Object.Instantiate(skillBGPrefab) as GameObject;
				skillBG.transform.parent = Map.Instance.BGPanel.transform;
				skillBG.transform.localPosition = new Vector3(0f, GameController.VIEW_HEIGHT / 2f, 0f);
				break;
			}
			if (skillBG != null)
			{
				Object.Destroy(skillBG);
				skillBG = null;
			}
			if (!skillBGPrefab)
			{
			}
			break;
		case 15:
			ballTf = GameController.Instance.ballAnim.gameObject.transform;
			GameController.Instance.ballAnim.Play(GameController.Instance.ballAnim.GetClipByName("ballClip"));
			ballTf.gameObject.SetActive(isLoad);
			break;
		case 16:
			missileTf1 = GameController.Instance.missileAnim1.gameObject.transform;
			missileTf1.gameObject.SetActive(value: false);
			missileTf2 = GameController.Instance.missileAnim2.gameObject.transform;
			missileTf2.gameObject.SetActive(value: false);
			break;
		}
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	public void InitPlayer(int id)
	{
		data = PlayerManager.Instance.GetDataByID(id);
		int playerLevel = User.Instance.GetPlayerLevel(id);
		state = STATE.ST_PLAY;
		base.transform.position = new Vector3(0.5f, 6f, 0f);
		lastX = 0f;
		isJump = false;
		activeSkillID = -1;
		maxWalkSpeed = (float)PlayerManager.Instance.GetPlayerSpeed(id, playerLevel) * 7f / 80f;
		maxFlySpeed = maxWalkSpeed * 6f / 7f;
		speedRate = 1f;
		h = 0f;
		isHeadHit = false;
		isGrounded = false;
		isTrans = false;
		isHammering = false;
		isBallShoot = false;
		isBallSkill = false;
		isMissile = false;
		base.rigidbody2D.isKinematic = false;
		base.rigidbody2D.gravityScale = GRAVITY_SCALE;
		base.rigidbody2D.velocity = new Vector2(0f, 0f);
		isIgnoreUpRock = false;
		Physics2D.IgnoreLayerCollision(Map.Instance.upRockLayer, Map.Instance.defaultLayer, ignore: false);
		Physics2D.IgnoreLayerCollision(Map.Instance.downRockLayer, Map.Instance.defaultLayer, ignore: false);
		skillEffectAnimator.Library = skillEffectAnimation;
		skillEffectAnimator.AnimationCompleted = null;
		skillEffectObject.SetActive(value: false);
		skillRemainText.gameObject.SetActive(value: false);
		currentID = id;
		UpdateByID();
		UpdateDataByID();
		LoadPlayer(isLoad: true);
	}

	public void EndPlay()
	{
		if (isLoaded)
		{
			jumpClip = playerAnimator.GetClipByName("jumpClip");
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			LoadPlayer(isLoad: false);
		}
	}

	private void PlayClip(tk2dSpriteAnimationClip clip, bool force = false)
	{
		if (playerAnimator.CurrentClip != clip)
		{
			if (playerAnimator.CurrentClip == jumpClip)
			{
				playerDustAnimator.transform.position = base.gameObject.transform.position;
				playerDustAnimator.gameObject.SetActive(value: true);
				playerDustAnimator.AnimationCompleted = DustAnimationCompleted;
				playerDustAnimator.Play();
			}
			playerAnimator.Play(clip);
		}
		else if (force)
		{
			playerAnimator.Play(clip);
		}
	}

	private void SkillAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		isSkillClip = false;
	}

	private void SkillHammerAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		isSkillClip = false;
		isHammering = false;
	}

	private void SkillSoccerAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		isSkillClip = false;
	}

	private void SkillMissileAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		isSkillClip = false;
	}

	private void SkillSAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		isSkillClip = false;
		ShowSkillEffect(4);
	}

	private void DustAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		a.gameObject.SetActive(value: false);
	}

	private void SafeAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		a.gameObject.SetActive(value: false);
	}

	public void ShowSafe()
	{
		if (state != STATE.ST_ZOMBIE)
		{
			playerSafeAnimator.transform.position = base.gameObject.transform.position + safeOffset;
			playerSafeAnimator.gameObject.SetActive(value: true);
			playerSafeAnimator.AnimationCompleted = SafeAnimationCompleted;
			playerSafeAnimator.Play();
		}
	}

	public void UpdatePlayer()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		lastX = base.gameObject.transform.position.x;
		if (state == STATE.ST_ZOMBIE || isSkillClip || isTeleport || isBallSkill)
		{
			if (isTeleport)
			{
				UpdateAnimDirection();
			}
			else
			{
				UpdateBall();
			}
			return;
		}
		if (base.rigidbody2D.velocity.x == 0f && base.rigidbody2D.velocity.y == 0f && h == 0f)
		{
			if (isHeadHit)
			{
				if (activeSkillID != 4 && activeSkillID != 9 && activeSkillID != 2 && activeSkillID != 14)
				{
					PlayClip(sitClip);
				}
			}
			else
			{
				PlayClip(stayClip);
			}
		}
		else if (isGrounded)
		{
			PlayClip(moveClip);
		}
		else if (activeSkillID == 8)
		{
			PlayClip(moveClip);
		}
		else
		{
			PlayClip(jumpClip);
		}
		UpdateBall();
		UpdateAnimDirection();
	}

	private void UpdateBall()
	{
		if (data.GetSkillID() == 15)
		{
			ballTf.localScale = new Vector3(playerSprite.scale.x, 1f, 1f);
			if (isBallShoot)
			{
				ballTf.position += new Vector3(ballVelX * Time.deltaTime, 12f * Time.deltaTime, 0f);
			}
			else if (playerAnimator.CurrentClip == jumpClip)
			{
				ballTf.position = base.transform.position + new Vector3(playerSprite.scale.x * 0.24f, 0.35f, 0f);
				GameController.Instance.ballAnim.Play();
			}
			else if (playerAnimator.CurrentClip == moveClip)
			{
				balldistX = Mathf.Sin(Time.time * 15f) * 0.1f + 0.1f;
				balldistY = 0f;
				ballTf.position = base.transform.position + new Vector3(playerSprite.scale.x * (0.3f + balldistX), 0.1f + balldistY, 0f);
				GameController.Instance.ballAnim.Play();
			}
			else if (playerAnimator.CurrentClip == stayClip)
			{
				ballTf.position = base.transform.position + new Vector3(playerSprite.scale.x * 0.22f, 0.07f, 0f);
				GameController.Instance.ballAnim.Stop();
			}
		}
		else if (data.GetSkillID() == 16 && isMissile)
		{
			missileTf1.position += new Vector3(0f, 6f * Time.deltaTime, 0f);
			missileTf2.position += new Vector3(5f * Time.deltaTime, 5f * Time.deltaTime, 0f);
		}
	}

	private void UpdateAnimDirection()
	{
		if (base.rigidbody2D.velocity.x > 0f)
		{
			playerSprite.scale = new Vector3(1f, 1f, 1f);
		}
		else if (base.rigidbody2D.velocity.x < 0f)
		{
			playerSprite.scale = new Vector3(-1f, 1f, 1f);
		}
	}

	public void UpdateFlyTarget(Rock rock)
	{
		flyTargetRock = rock;
		float num = flyTargetRock.transform.position.y + (float)flyTargetRock.GetGridHeight() * Map.GRID_SCALE * 0.5f;
		float num2 = (num + 0.5f + 12f * Map.GRID_SCALE * 0.5f) * 0.5f;
		float num3 = flyTargetRock.transform.position.x - base.transform.position.x;
		float num4 = num2 - base.transform.position.y;
		flyVelX = skillFlySpeed * num3 / Mathf.Sqrt(num3 * num3 + num4 * num4);
		flyVelY = skillFlySpeed * num4 / Mathf.Sqrt(num3 * num3 + num4 * num4);
		flyTargetObject.transform.position = new Vector3(flyTargetRock.transform.position.x, num2, flyTargetRock.transform.position.z);
	}

	public void UpdateTeleportTarget()
	{
		Rock collideRock = Map.Instance.GetCollideRock(isUp: false);
		flyTargetRock = collideRock.rightRock;
		for (int i = 0; i < 8; i++)
		{
			if (!(flyTargetRock.rightRock != null))
			{
				break;
			}
			if (!flyTargetRock.rightRock.IsActive())
			{
				break;
			}
			flyTargetRock = flyTargetRock.rightRock;
		}
		float num = flyTargetRock.transform.position.y + (float)flyTargetRock.GetGridHeight() * Map.GRID_SCALE * 0.5f;
		float num2 = num;
		float num3 = flyTargetRock.transform.position.x - base.transform.position.x;
		float num4 = num2 - base.transform.position.y;
		flyTargetVector = new Vector3(flyTargetRock.transform.position.x, num2, 0f);
		flyVelX = skillTeleportSpeed * num3 / Mathf.Sqrt(num3 * num3 + num4 * num4);
		flyVelY = skillTeleportSpeed * num4 / Mathf.Sqrt(num3 * num3 + num4 * num4);
		flyTargetObject.transform.position = new Vector3(flyTargetRock.transform.position.x, num2, flyTargetRock.transform.position.z);
		teleportCount++;
	}

	private void FixedUpdate()
	{
		isGrounded = false;
		isHeadHit = false;
		if (state == STATE.ST_DEAD || state == STATE.ST_ZOMBIE)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		IngameUI.Instance.CheckMoveControl();
		if (activeSkillID == 8)
		{
			if (IngameUI.Instance.isLeft)
			{
				h = 0f;
			}
			else if (IngameUI.Instance.isRight)
			{
				h = 1f;
			}
			else
			{
				h = 0f;
			}
			if (IsOut())
			{
				SetDead();
				return;
			}
			if ((bool)flyTargetRock && base.transform.position.x > flyTargetRock.gameObject.transform.position.x)
			{
				UpdateFlyTarget(flyTargetRock.rightRock);
			}
			base.rigidbody2D.velocity = new Vector2(flyVelX * h, flyVelY * h);
			return;
		}
		if (activeSkillID == 5)
		{
			if (IsOut())
			{
				SetDead();
			}
			else
			{
				if (!isTeleportMoving)
				{
					return;
				}
				base.rigidbody2D.velocity = new Vector2(flyVelX, flyVelY);
				if (base.transform.position.x >= flyTargetVector.x)
				{
					base.transform.position = flyTargetVector;
					if (teleportCount >= 2)
					{
						OnTeleportMovingEnd(showAnim: true);
					}
					else
					{
						UpdateTeleportTarget();
					}
				}
			}
			return;
		}
		RaycastHit2D raycastHit2D = Physics2D.Linecast(base.transform.position, base.transform.position + groundLeft, 1 << Map.Instance.downRockLayer);
		RaycastHit2D raycastHit2D2 = Physics2D.Linecast(base.transform.position, base.transform.position + groundRight, 1 << Map.Instance.downRockLayer);
		isGrounded = (bool)raycastHit2D || (bool)raycastHit2D2;
		RaycastHit2D raycastHit2D3 = Physics2D.Linecast(base.transform.position, base.transform.position + headCheck, 1 << Map.Instance.upRockLayer);
		RaycastHit2D raycastHit2D4 = Physics2D.Linecast(base.transform.position + groundCenter, base.transform.position + deadCheck, 1 << Map.Instance.upRockLayer);
		RaycastHit2D raycastHit2D5;
		RaycastHit2D raycastHit2D6;
		if (isBallShoot)
		{
			raycastHit2D5 = Physics2D.Linecast(ballTf.position, ballTf.position + new Vector3(0f, 0.1f, 0f), 1 << Map.Instance.upRockLayer);
			raycastHit2D6 = default(RaycastHit2D);
		}
		else if (isMissile)
		{
			raycastHit2D5 = Physics2D.Linecast(missileTf1.position, missileTf1.position + new Vector3(0f, 0.1f, 0f), 1 << Map.Instance.upRockLayer);
			raycastHit2D6 = Physics2D.Linecast(base.transform.position, missileTf2.position + new Vector3(0f, 0.1f, 0f), 1 << Map.Instance.upRockLayer);
		}
		else
		{
			raycastHit2D5 = default(RaycastHit2D);
			raycastHit2D6 = default(RaycastHit2D);
		}
		isHeadHit = raycastHit2D3;
		if (isTrans)
		{
			isHeadHit = false;
		}
		bool flag = IsOut();
		if ((bool)raycastHit2D4)
		{
			if (isGrounded && !isTrans)
			{
				if (activeSkillID == 2)
				{
					Rock component = raycastHit2D4.collider.gameObject.GetComponent<Rock>();
					if ((bool)component)
					{
						component.Demolish();
						Map.Instance.CreateRockFrag(base.transform.position.x - 0.5f, base.transform.position.y + 2f);
						Map.Instance.CreateRockFrag(base.transform.position.x + 0.5f, base.transform.position.y + 2f);
						Map.Instance.SetUpRockSlide(val: false);
					}
				}
				else
				{
					if (activeSkillID != 14)
					{
						if (activeSkillID == 6)
						{
							SetZombie();
						}
						else
						{
							SetDead();
						}
						return;
					}
					if (!isSkillClip)
					{
						DoHulk();
					}
				}
			}
		}
		else if (isHeadHit && isGrounded)
		{
			if (activeSkillID == 2)
			{
				Rock component2 = raycastHit2D3.collider.gameObject.GetComponent<Rock>();
				if ((bool)component2)
				{
					component2.Demolish();
					Map.Instance.CreateRockFrag(base.transform.position.x - 0.5f, base.transform.position.y + 2f);
					Map.Instance.CreateRockFrag(base.transform.position.x + 0.5f, base.transform.position.y + 2f);
				}
			}
			else if (activeSkillID == 14)
			{
				DoHulk();
			}
		}
		if ((bool)raycastHit2D5.collider)
		{
			Rock component3 = raycastHit2D5.collider.gameObject.GetComponent<Rock>();
			if ((bool)component3)
			{
				Util.d("demolish ball");
				component3.DemolishUp();
				Transform transform = ((!isBallShoot) ? missileTf1 : ballTf);
				Map.Instance.CreateRockFrag(transform.position.x - 0.5f, transform.position.y);
				Map.Instance.CreateRockFrag(transform.position.x + 0.5f, transform.position.y);
				if (isBallShoot)
				{
					isBallShoot = false;
					isBallSkill = false;
					StartCoroutine(ChangeBall());
				}
				if (isMissile)
				{
					missileTf1.gameObject.SetActive(value: false);
					if (!missileTf1.gameObject.activeSelf && !missileTf2.gameObject.activeSelf)
					{
						isMissile = false;
					}
				}
			}
		}
		if ((bool)raycastHit2D6.collider)
		{
			Rock component4 = raycastHit2D6.collider.gameObject.GetComponent<Rock>();
			if ((bool)component4)
			{
				Util.d("demolish ball");
				component4.DemolishUp();
				Transform transform2 = missileTf2;
				Map.Instance.CreateRockFrag(transform2.position.x - 0.5f, transform2.position.y);
				Map.Instance.CreateRockFrag(transform2.position.x + 0.5f, transform2.position.y);
				missileTf2.gameObject.SetActive(value: false);
				if (!missileTf1.gameObject.activeSelf && !missileTf2.gameObject.activeSelf)
				{
					isMissile = false;
				}
			}
		}
		if (flag)
		{
			SetDead();
			return;
		}
		if (!isHeadHit && isGrounded && IngameUI.Instance.isJump)
		{
			isJump = true;
			jumpDuration = Time.deltaTime;
		}
		else
		{
			isJump = false;
			jumpDuration = 0f;
		}
		if (isHeadHit)
		{
			h = 0f;
		}
		else if (IngameUI.Instance.isLeft)
		{
			h = -1f;
		}
		else if (IngameUI.Instance.isRight)
		{
			h = 1f;
		}
		else
		{
			h = 0f;
		}
		if (isJump)
		{
			num2 = jumpForce;
			isJump = false;
			IngameUI.Instance.isJump = false;
			GameController.Instance.PlaySound(AudioManager.Instance.jump);
		}
		moveVel = ((!isGrounded) ? maxFlySpeed : maxWalkSpeed) * speedRate;
		if (num2 != 0f)
		{
			base.rigidbody2D.AddForce(new Vector2(0f, num2));
		}
		base.rigidbody2D.velocity = new Vector2(moveVel * h, base.rigidbody2D.velocity.y);
		if ((isHeadHit && isGrounded) || isTrans)
		{
			IgnoreUpRock(ignore: true);
		}
		else
		{
			IgnoreUpRock(ignore: false);
		}
		if (!IngameUI.Instance.isSkill)
		{
			return;
		}
		if (data.GetSkillID() == 12 && !isHammering)
		{
			isHammering = true;
			PlayClip(playerAnimator.GetClipByName("skillClip"), force: true);
			isSkillClip = true;
			playerAnimator.AnimationCompleted = SkillHammerAnimationCompleted;
			GameController.Instance.PlaySound(AudioManager.Instance.hammer);
			RaycastHit2D raycastHit2D7 = Physics2D.Linecast(base.transform.position, base.transform.position + groundCenter2, 1 << Map.Instance.downRockLayer);
			if ((bool)raycastHit2D7)
			{
				Rock rock = raycastHit2D7.collider.gameObject.GetComponent<Rock>();
				if ((bool)rock)
				{
					playerAnimator.AnimationEventTriggered = delegate(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2, int frames)
					{
						if (frames == 4)
						{
							rock.DemolishDown();
							Map.Instance.CreateRockFrag(base.transform.position.x - 0.5f, base.transform.position.y - 0.5f);
							Map.Instance.CreateRockFrag(base.transform.position.x + 0.5f, base.transform.position.y - 0.5f);
						}
					};
				}
			}
		}
		else if (data.GetSkillID() == 15 && !isBallSkill)
		{
			isBallSkill = true;
			isBallShoot = false;
			playerAnimator.Stop();
			PlayClip(playerAnimator.GetClipByName("skillClip"), force: true);
			isSkillClip = true;
			ballTf.gameObject.SetActive(value: false);
			GameController.Instance.PlaySound(AudioManager.Instance.soccer);
			playerAnimator.AnimationCompleted = SkillSoccerAnimationCompleted;
			playerAnimator.AnimationEventTriggered = delegate(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2, int frames)
			{
				if (frames == 4 && !isBallShoot)
				{
					ballTf.gameObject.SetActive(value: true);
					isBallShoot = true;
					if (playerSprite.scale.x > 0f)
					{
						ballVelX = 12f;
					}
					else
					{
						ballVelX = 0f;
					}
					GameController.Instance.ballAnim.Play("ballskillClip");
					ballTf.position = base.transform.position + new Vector3(playerSprite.scale.x * 0.26f, 0.35f, 0f);
				}
			};
		}
		else if (data.GetSkillID() == 16 && !isMissile)
		{
			Debug.Log("Missile!!!");
			isMissile = true;
			missileTf1.gameObject.SetActive(value: true);
			missileTf1.position = base.transform.position + new Vector3(0f, 1f, 0f);
			missileTf2.gameObject.SetActive(value: true);
			missileTf2.position = base.transform.position + new Vector3(1f, 1f, 0f);
			PlayClip(playerAnimator.GetClipByName("skillClip"), force: true);
			isSkillClip = true;
			playerAnimator.AnimationCompleted = SkillMissileAnimationCompleted;
			GameController.Instance.PlaySound(AudioManager.Instance.missile);
		}
		IngameUI.Instance.isSkill = false;
	}

	private IEnumerator ChangeBall()
	{
		yield return new WaitForEndOfFrame();
		GameController.Instance.ballAnim.Play(GameController.Instance.ballAnim.GetClipByName("ballClip"));
	}

	private void Teleport()
	{
		if (!isTeleport)
		{
			teleportCount = 0;
			UpdateTeleportTarget();
			Util.d("teleport");
			isTeleport = true;
			PlayClip(stayClip);
			skillEffectAnimator.gameObject.SetActive(value: true);
			skillEffectAnimator.Library = playerAnimator.Library;
			skillEffectAnimator.Play("skill1Clip");
			skillEffectObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			skillEffectAnimator.AnimationCompleted = Teleport1AnimationCompleted;
			base.renderer.enabled = false;
		}
	}

	private void Teleport1AnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		Util.d("teleport 1 comp");
		skillEffectAnimator.gameObject.SetActive(value: false);
		isTeleportMoving = true;
		base.rigidbody2D.isKinematic = true;
		base.rigidbody2D.gravityScale = 0f;
	}

	private void OnTeleportMovingEnd(bool showAnim)
	{
		Util.d("OnTeleportMovingEnd");
		if (showAnim)
		{
			skillEffectAnimator.gameObject.SetActive(value: true);
			skillEffectAnimator.Library = playerAnimator.Library;
			skillEffectAnimator.Play("skill2Clip");
			skillEffectObject.transform.localPosition = new Vector3(0f, 0.5f, 0f);
			skillEffectAnimator.AnimationCompleted = Teleport2AnimationCompleted;
		}
		else
		{
			skillEffectAnimator.gameObject.SetActive(value: false);
			isTeleport = false;
		}
		base.rigidbody2D.isKinematic = false;
		base.rigidbody2D.gravityScale = GRAVITY_SCALE;
		isTeleportMoving = false;
		base.rigidbody2D.velocity = new Vector2(0f, 0f);
	}

	private void Teleport2AnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		Util.d("teleport 2 comp");
		isTeleport = false;
		skillEffectAnimator.gameObject.SetActive(value: false);
		base.renderer.enabled = true;
		GameController.Instance.EndSkill(5);
	}

	public bool IsDead()
	{
		return state == STATE.ST_DEAD;
	}

	public bool IsOut()
	{
		return (float)Map.Instance.baseY - base.transform.position.y > (float)GameController.SCREEN_HEIGHT / GameController.CAMERA_SCALE / 2f;
	}

	private void SetDead()
	{
		state = STATE.ST_DEAD;
		base.rigidbody2D.velocity = Vector2.zero;
		IgnoreUpRock(ignore: true);
		Util.d("DEAD");
		GameController.Instance.Vibrate();
		GameController.Instance.PlaySound(AudioManager.Instance.die, 0.5f);
	}

	private void SetSpeedUp()
	{
		PlayClip(playerAnimator.GetClipByName("skillClip"));
		isSkillClip = true;
		playerAnimator.AnimationCompleted = SkillSAnimationCompleted;
		jumpClip = playerAnimator.GetClipByName("skillJumpClip");
		stayClip = playerAnimator.GetClipByName("skillStayClip");
		moveClip = playerAnimator.GetClipByName("skillMoveClip");
		SetSpeedRate(2.5f);
	}

	private void SetSpeedUp2()
	{
		jumpClip = playerAnimator.GetClipByName("skillJumpClip");
		stayClip = playerAnimator.GetClipByName("skillStayClip");
		moveClip = playerAnimator.GetClipByName("skillMoveClip");
		SetSpeedRate(2.5f);
	}

	private void SetSlow(bool isSlow)
	{
		Map.Instance.SetSlow(isSlow);
	}

	private void Rebirth0()
	{
		Util.d("Rebirth0");
		playerAnimator.Play();
		playerAnimator.AnimationCompleted = RebirthAnimationCompleted;
	}

	private void RebirthAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		playerAnimator.AnimationCompleted = null;
		Rebirth();
	}

	private void Rebirth()
	{
		state = STATE.ST_PLAY;
		IgnoreUpRock(ignore: false);
		Util.d("ZOMBIE REBIRTH");
		GameController.Instance.EndSkill(6);
	}

	private void SetZombie()
	{
		if (state == STATE.ST_PLAY)
		{
			state = STATE.ST_ZOMBIE;
			base.rigidbody2D.velocity = Vector2.zero;
			IgnoreUpRock(ignore: true);
			Util.d("DEAD ZOMBIE");
			skillEffectObject.SetActive(value: false);
			PlayClip(playerAnimator.GetClipByName("skillClip"));
			playerAnimator.Stop();
			Invoke("Rebirth0", 2f);
			Map.Instance.SetExtraStopDuration(4f);
			Map.Instance.SetUpRockSlide(val: false);
			GameController.Instance.PlaySound(AudioManager.Instance.die, 0.5f);
		}
	}

	public bool IsSkillActive()
	{
		return activeSkillID >= 1;
	}

	public int GetActiveSkill()
	{
		return activeSkillID;
	}

	public void SetActiveSkill(int skillID)
	{
		if (skillID != activeSkillID)
		{
			EndSkill(activeSkillID);
			activeSkillID = skillID;
			StartSkill(activeSkillID);
			Util.d("SetActiveSkill " + skillID);
		}
	}

	private void ShowSkillEffect(int skillID = -1)
	{
		skillEffectAnimator.gameObject.SetActive(value: true);
		switch (skillID)
		{
		case -1:
			skillEffectAnimator.Play("skillStartClip");
			skillEffectAnimator.AnimationCompleted = SkillEffectAnimationCompleted;
			skillEffectObject.transform.localPosition = new Vector3(0f, 0.4f, 0f);
			break;
		case 7:
			skillEffectAnimator.Library = playerAnimator.Library;
			skillEffectAnimator.Play("skillClip");
			skillEffectAnimator.AnimationCompleted = SkillEffectAnimationCompleted;
			skillEffectObject.transform.localPosition = new Vector3(0f, 1.2f, 0f);
			break;
		case 6:
			skillEffectAnimator.Library = playerAnimator.Library;
			skillEffectAnimator.Play("skillFxClip");
			skillEffectObject.transform.localPosition = new Vector3(0f, 1.1f, 0f);
			skillEffectAnimator.AnimationCompleted = null;
			break;
		case 4:
			skillEffectAnimator.Library = playerAnimator.Library;
			skillEffectAnimator.Play("skillFxClip");
			skillEffectAnimator.AnimationCompleted = SkillEffectAnimationCompleted;
			skillEffectObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			break;
		case 9:
			skillEffectAnimator.Play("skillStartClip");
			skillEffectAnimator.AnimationCompleted = SkillEffectAnimationCompleted;
			skillEffectObject.transform.localPosition = new Vector3(0f, 0.4f, 0f);
			break;
		case 10:
			skillEffectAnimator.Play("skillStartClip");
			skillEffectAnimator.AnimationCompleted = delegate
			{
				skillEffectAnimator.Library = playerAnimator.Library;
				skillEffectAnimator.Play("skillClip");
				skillEffectAnimator.AnimationCompleted = null;
				skillEffectObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			};
			break;
		case 14:
			skillEffectAnimator.Play("skillStartClip");
			skillEffectAnimator.AnimationCompleted = SkillEffectAnimationCompleted;
			skillEffectObject.transform.localPosition = new Vector3(0f, 0.4f, 0f);
			break;
		}
	}

	private void SkillEffectAnimationCompleted(tk2dSpriteAnimator a, tk2dSpriteAnimationClip c)
	{
		a.gameObject.SetActive(value: false);
	}

	public void UpdateSkill()
	{
		float num = GameController.Instance.skillDuration;
		if (num < 0f)
		{
			num = 0f;
		}
		if (num <= 5f)
		{
			skillRemainText.text = Mathf.Floor(num).ToString();
			skillRemainText.gameObject.SetActive(value: true);
		}
		else
		{
			skillRemainText.gameObject.SetActive(value: false);
		}
	}

	private void StartSkill(int skillID)
	{
		if (skillID != -1)
		{
			GameController.Instance.PlaySound(AudioManager.Instance.skill);
		}
		switch (skillID)
		{
		case -1:
			break;
		case 6:
			ShowSkillEffect(skillID);
			break;
		case 3:
			ShowSkillEffect();
			jumpClip = playerAnimator.GetClipByName("skillJumpClip");
			stayClip = playerAnimator.GetClipByName("skillStayClip");
			moveClip = playerAnimator.GetClipByName("skillMoveClip");
			break;
		case 1:
			skillBG.gameObject.SetActive(value: true);
			Map.Instance.Blink();
			break;
		case 13:
			ShowSkillEffect();
			Map.Instance.Blink();
			stayClip = playerAnimator.GetClipByName("skillStayClip");
			moveClip = playerAnimator.GetClipByName("skillMoveClip");
			jumpClip = moveClip;
			break;
		case 4:
			isSkillClip = true;
			Invoke("SetSpeedUp", 0.3f);
			break;
		case 9:
			ShowSkillEffect(skillID);
			Invoke("SetSpeedUp2", 0.1f);
			break;
		case 8:
			ShowSkillEffect();
			stayClip = playerAnimator.GetClipByName("skillStayClip");
			moveClip = playerAnimator.GetClipByName("skillMoveClip");
			base.rigidbody2D.isKinematic = true;
			base.rigidbody2D.gravityScale = 0f;
			break;
		case 5:
			Teleport();
			break;
		case 2:
			ShowSkillEffect();
			jumpClip = playerAnimator.GetClipByName("skillJumpClip");
			stayClip = playerAnimator.GetClipByName("skillStayClip");
			moveClip = playerAnimator.GetClipByName("skillMoveClip");
			break;
		case 7:
			ShowSkillEffect(skillID);
			break;
		case 10:
			ShowSkillEffect(10);
			SetSlow(isSlow: true);
			break;
		case 11:
			SetTrans(tr: true);
			break;
		case 12:
			ShowSkillEffect();
			jumpClip = playerAnimator.GetClipByName("skillJumpClip");
			stayClip = playerAnimator.GetClipByName("skillStayClip");
			moveClip = playerAnimator.GetClipByName("skillMoveClip");
			break;
		case 14:
			ShowSkillEffect(skillID);
			jumpClip = playerAnimator.GetClipByName("skillJumpClip");
			stayClip = playerAnimator.GetClipByName("skillStayClip");
			moveClip = playerAnimator.GetClipByName("skillMoveClip");
			break;
		case 15:
			break;
		case 16:
			ShowSkillEffect();
			jumpClip = playerAnimator.GetClipByName("skillJumpClip");
			stayClip = playerAnimator.GetClipByName("skillStayClip");
			moveClip = playerAnimator.GetClipByName("skillMoveClip");
			sitClip = playerAnimator.GetClipByName("skillSitClip");
			break;
		case 0:
			break;
		}
	}

	private void EndSkill(int prevSkillID)
	{
		skillRemainText.gameObject.SetActive(value: false);
		switch (prevSkillID)
		{
		case -1:
			break;
		case 3:
			jumpClip = playerAnimator.GetClipByName("jumpClip");
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			break;
		case 1:
			skillBG.gameObject.SetActive(value: false);
			break;
		case 13:
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			jumpClip = moveClip;
			break;
		case 4:
		case 9:
			jumpClip = playerAnimator.GetClipByName("jumpClip");
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			SetSpeedRate(1f);
			break;
		case 8:
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			base.rigidbody2D.gravityScale = GRAVITY_SCALE;
			base.rigidbody2D.isKinematic = false;
			break;
		case 5:
			if (isTeleportMoving)
			{
				base.transform.position = flyTargetVector;
			}
			OnTeleportMovingEnd(showAnim: false);
			base.renderer.enabled = true;
			break;
		case 2:
			jumpClip = playerAnimator.GetClipByName("jumpClip");
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			break;
		case 10:
			skillEffectAnimator.gameObject.SetActive(value: false);
			SetSlow(isSlow: false);
			break;
		case 11:
			SetTrans(tr: false);
			break;
		case 12:
			jumpClip = playerAnimator.GetClipByName("jumpClip");
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			break;
		case 14:
			jumpClip = playerAnimator.GetClipByName("jumpClip");
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			break;
		case 15:
			break;
		case 16:
			jumpClip = playerAnimator.GetClipByName("jumpClip");
			stayClip = playerAnimator.GetClipByName("stayClip");
			moveClip = playerAnimator.GetClipByName("moveClip");
			sitClip = playerAnimator.GetClipByName("sitClip");
			break;
		case 0:
		case 6:
		case 7:
			break;
		}
	}

	private void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "GEM")
		{
			Gem component = coll.gameObject.GetComponent<Gem>();
			if ((bool)component && component.Hit())
			{
				GameController.Instance.PlaySound(AudioManager.Instance.get);
				GameController.Instance.AddGem(component.GetID(), 1);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "GEM")
		{
			Gem component = coll.gameObject.GetComponent<Gem>();
			if ((bool)component && component.Hit())
			{
				GameController.Instance.PlaySound(AudioManager.Instance.get);
				GameController.Instance.AddGem(component.GetID(), 1);
			}
		}
		else if (coll.gameObject.tag == "GEMBOX")
		{
			GemBox component2 = coll.gameObject.GetComponent<GemBox>();
			if ((bool)component2)
			{
				component2.Hit();
			}
		}
	}

	private void SetSpeedRate(float speedRate)
	{
		this.speedRate = speedRate;
	}

	private void SetTrans(bool tr)
	{
		Util.d("SetTrans " + tr);
		isTrans = tr;
		if (tr)
		{
			IgnoreUpRock(ignore: true);
			playerSprite.color = new Color(1f, 1f, 1f, 0.4f);
			playerSprite.SortingOrder = 1;
		}
		else
		{
			IgnoreUpRock(ignore: false);
			playerSprite.color = new Color(1f, 1f, 1f, 1f);
			playerSprite.SortingOrder = 0;
		}
	}

	private void IgnoreUpRock(bool ignore)
	{
		if (isIgnoreUpRock != ignore)
		{
			isIgnoreUpRock = ignore;
			Physics2D.IgnoreLayerCollision(Map.Instance.upRockLayer, Map.Instance.defaultLayer, ignore);
		}
	}

	private void DoHulk()
	{
		isSkillClip = true;
		PlayClip(playerAnimator.GetClipByName("skillClip"));
		playerAnimator.AnimationCompleted = SkillAnimationCompleted;
		Map.Instance.DoHulk();
		playerDustAnimator.transform.position = base.gameObject.transform.position + new Vector3(0f, 1f, 0f);
		playerDustAnimator.gameObject.SetActive(value: true);
		playerDustAnimator.AnimationCompleted = DustAnimationCompleted;
		playerDustAnimator.Play();
	}
}
