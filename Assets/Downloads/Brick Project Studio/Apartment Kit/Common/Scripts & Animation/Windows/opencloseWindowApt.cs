using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles

{
	public class opencloseWindowApt : MonoBehaviour
	{
        public Animator openandclosewindow;
        public bool open;
        public Transform Player;

        [Header("Звук")]
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool startActive = false;

        [Header("визуальная обратная связь (не обязательно)")]
        [SerializeField] private Renderer objectRenderer;
        [SerializeField] private Color activeColor = new Color(0.3f, 1f, 0.3f);
        [SerializeField] private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f);
        [SerializeField] private float fadeSpeed = 4f;


        private AudioSource audioSource;
        private bool isActive;
        private float targetVolume;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0f;
        }

        void Update()
        {
            if (!Mathf.Approximately(audioSource.volume, targetVolume))
            {
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            }
        }

        public void Interact()
        {
            SetActive(!isActive);
        }

        public string GetPrompt()
        {
            return isActive ? "выключить" : "включить";
        }

        private void SetActive(bool active, bool instant = false, bool notify = true)
        {
            isActive = active;
            targetVolume = active ? 1f : 0f;

            if (instant)
            {
                audioSource.volume = targetVolume;
            }
            UpdateVisual();

            if (notify)
            {
                MusicManager.Instance.NotifyLayerToggled(active);
            }
        }

        private void UpdateVisual()
        {
            if (objectRenderer != null)
            {
                objectRenderer.material.color = isActive ? activeColor : inactiveColor;
            }
        }

		void Start()
		{
            MusicManager.Instance.RegisterLayer(audioSource, startActive);
            SetActive(startActive, instant: true, notify: false);
            open = false;
		}

		void OnMouseOver()
		{
			{
				if (Player)
				{
					float dist = Vector3.Distance(Player.position, transform.position);
					if (dist < 15)
					{
						if (open == false)
						{
							if (Input.GetKeyDown(KeyCode.E))
							{
								StartCoroutine(opening());
                                SetActive(!isActive, instant: true, notify: false);
                                open = false;
							}
						}
						else
						{
							if (open == true)
							{
								if (Input.GetKeyDown(KeyCode.E))
								{
									StartCoroutine(closing());
								}
							}

						}

					}
				}

			}

		}

		IEnumerator opening()
		{
			print("you are opening the Window");
			openandclosewindow.Play("Openingwindow");
			open = true;
			yield return new WaitForSeconds(.5f);
		}

		IEnumerator closing()
		{
			print("you are closing the Window");
			openandclosewindow.Play("Closingwindow");
			open = false;
			yield return new WaitForSeconds(.5f);
		}


	}
}