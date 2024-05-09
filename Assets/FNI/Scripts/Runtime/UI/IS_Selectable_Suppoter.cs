/// 작성자: 백인성
/// 작성일: 2020-04-14
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI
{
    public class IS_Selectable_Suppoter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Selectable MySelectable
        {
            get
            {
                if (m_selectable == null)
                    m_selectable = GetComponent<Selectable>();

                return m_selectable;
            }
        }
        private AudioSource Audio
        {
            get
            {
                if (m_audioSource == null)
                    m_audioSource = GetComponent<AudioSource>();
                if (m_audioSource == null)
                    m_audioSource = gameObject.AddComponent<AudioSource>();

                if (m_audioSource != null && audios.Init == false)
                {
                    audios.Init = true;
                    m_audioSource.playOnAwake = false;
                    m_audioSource.outputAudioMixerGroup = audios.audioMixer;
                    m_audioSource.spatialBlend = audios.is3DAudio ? 1 : 0;
                    m_audioSource.volume = audios.audioVolume;
                    m_audioSource.minDistance = audios.soundDistance.Min;
                    m_audioSource.maxDistance = audios.soundDistance.Max;
                }
                return m_audioSource;
            }
        }

        public bool interactable
        {
            get
            {
                //Debug.Log($"[get] - {MySelectable.name}/ {MySelectable.interactable}");
                return MySelectable.interactable;
            }
            set
            {
                MySelectable.interactable = value;
                Debug.Log($"{isEnter}/ {MySelectable.interactable} = {value} ?");

                if (value)
                {
                    state = isEnter ? AnimationState.Enter : AnimationState.Default;
                }
                else
                {
                    state = AnimationState.Disable;
                }

                SetSprite();
                InitValue();
            }
        }

        public IS_Selectables selectableRoot;

        public float lerpSpeed = 15;

        public AnimationSetAudio audios;
        public List<AnimationValues> animationValues;
        public List<Graphic> graphics;
        public List<Text> texts;
        public string innerTexts(int num) { return texts[num].text; }
        public string FirstTexts { get => (texts == null || texts.Count == 0 | texts [0] == null) ? "" : texts [0].text; }

        private AudioSource m_audioSource;
        public AnimationState state;
        private bool isEnter = false;
        private Selectable m_selectable;

        private RectTransform m_bg;
              
        private void OnEnable()
        {
            selectableRoot = this.GetComponentInParent<IS_Selectables>();
            if (selectableRoot == null)
            {
                Debug.Log("selectableRoot가 없어요! 넣어주세요.");
                return;
            }
            selectableRoot.AddSupport(this);

            if (interactable)
            {
                state = AnimationState.Default;

                SetSprite();
                InitValue();
            }

            if (texts == null || texts.Count == 0)
                IS_Func.GetComponentsInChildren(texts, transform);
            if (texts == null || texts.Count == 0)
                IS_Func.GetComponentsInChildren(graphics, transform);
        }

        private void OnDisable()
        {
            if (selectableRoot == null)
            {
                return;
            }
            selectableRoot.RemoveSupport(this);
        }

        private void InitValue()
        {
            for (int cnt = 0; cnt < animationValues.Count; cnt++)
            {
                animationValues[cnt].Init(state);
            }
        }
        public void Animation()
        {
            if (gameObject.activeSelf)
            {
                for (int cnt = 0; cnt < animationValues.Count; cnt++)
                {
                    animationValues[cnt].Animation(state, Time.deltaTime * lerpSpeed);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isEnter = true;

            if (interactable == false) return;

            state = AnimationState.Enter;

            SetAudio();
            SetSprite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isEnter = false;

            if (interactable == false) return;

            state = AnimationState.Default;

            SetAudio();
            SetSprite();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (interactable == false) return;

            state = AnimationState.Click;

            SetAudio();
            SetSprite();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (interactable == false) return;
            
            state = isEnter ? AnimationState.Enter : AnimationState.Default;

            SetSprite();
        }

        public void SetSprite()
        {
            for (int cnt = 0; cnt < animationValues.Count; cnt++)
            {
                if (animationValues[cnt].Sprite.Use)
                    animationValues[cnt].SetSprite(state);
            }
        }
        private void SetAudio()
        {
            if (audios.Use)
            {
                Audio.clip = audios.Get(state);
                if (Audio.clip != null)
                    Audio.Play();
            }
        }
    }

    public enum AnimationState
    {
        Default,
        Enter,
        Click,
        Disable,
    }

    [System.Serializable]
    public class AnimationValues
    {
        public RectTransform rect;
        public Image image;
        public Graphic graphic;
        public AnimationSetSprite Sprite;
        public AnimationSetColor Color;
        public AnimationSetVector3 Position;
        public AnimationSetVector2 Sclale;

        private Vector2 size;

        public void SetSprite(AnimationState state)
        {
            //Debug.Log($"<color=yellow> {state} </color>");
            if (image != null)
            {
                switch (state)
                {
                    case AnimationState.Default:
                        image.sprite = Sprite.Default;
                        break;
                    case AnimationState.Enter:
                        image.sprite = Sprite.Enter;
                        break;
                    case AnimationState.Click:
                        image.sprite = Sprite.Click;
                        break;
                    case AnimationState.Disable:
                        image.sprite = Sprite.Disable;
                        break;
                    default:
                        goto case AnimationState.Default;
                }
            }
        }
        public void Init(AnimationState state)
        {
            if (Color.Use)
            {
                if (graphic != null)
                    graphic.color = Color.Get(state);
            }
            if (Position.Use)
            {
                rect.anchoredPosition3D = Position.Get(state);
            }
            if (Sclale.Use)
            {
                if (Sclale.percent == false)
                    rect.sizeDelta = Sclale.Get(state);
            }
            

            if (size == Vector2.zero && image != null && image.sprite != null)
                size = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
        }

        public void Animation(AnimationState state, float lerpTime)
        {
            if (Color.Use)
            {
                if (graphic != null)
                    graphic.color = UnityEngine.Color.Lerp(graphic.color, Color.Get(state), lerpTime);
            }
            if (Position.Use)
            {
                rect.anchoredPosition3D = Vector3.Lerp(rect.anchoredPosition3D, Position.Get(state), lerpTime);
            }
            if (Sclale.Use)
            {
                if (Sclale.percent)
                {
                    if (Sclale.localScale)
                        rect.transform.localScale = Vector3.Lerp(rect.transform.localScale, Vector3.one * Sclale.Get_F(state), lerpTime);
                    else
                        rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, size * Sclale.Get_F(state), lerpTime);
                }
                else
                {
                    rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, Sclale.Get(state), lerpTime);
                }
            }
        }
    }

    //에디터에서 정상 작동 안함
    [System.Serializable]
    public class AnimationSet<T>
    {
        public bool Use;
        public T Default;
        public T Enter;
        public T Click;
        public T Disable;

        public T Get(AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Default:
                    return Default;
                case AnimationState.Enter:
                    return Enter;
                case AnimationState.Click:
                    return Click;
                case AnimationState.Disable:
                    return Disable;
                default:
                    goto case AnimationState.Default;
            }
        }
    }
    [System.Serializable]
    public class AnimationSetAudio
    {
        public bool Use;
        public AudioClip Enter;
        public AudioClip Exit;
        public AudioClip Click;

        public bool Init = false;
        public bool is3DAudio = true;
        public float audioVolume = 1;
        public MinMax soundDistance = new MinMax(0, 1);
        public AudioMixerGroup audioMixer;

        public AudioClip Get(AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Default:
                    return Exit;
                case AnimationState.Enter:
                    return Enter;
                case AnimationState.Click:
                    return Click;
                default:
                    goto case AnimationState.Default;
            }
        }
    }

    [System.Serializable]
    public class AnimationSetSprite
    {
        public bool Use;
        public Sprite Default;
        public Sprite Enter;
        public Sprite Click;
        public Sprite Disable;

        public Sprite Get(AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Default:
                    return Default;
                case AnimationState.Enter:
                    return Enter;
                case AnimationState.Click:
                    return Click;
                case AnimationState.Disable:
                    return Disable;
                default:
                    goto case AnimationState.Default;
            }
        }
    }

    [System.Serializable]
    public class AnimationSetColor
    {
        public bool Use;
        public Image target;
        public Color Default;
        public Color Enter;
        public Color Click;
        public Color Disable;

        public Color Get(AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Default:
                    return Default;
                case AnimationState.Enter:
                    return Enter;
                case AnimationState.Click:
                    return Click;
                case AnimationState.Disable:
                    return Disable;
                default:
                    goto case AnimationState.Default;
            }
        }
    }

    [System.Serializable]
    public class AnimationSetVector2
    {
        public bool Use;
        public bool percent;
        public bool localScale;
        public Vector2 Default;
        public Vector2 Enter;
        public Vector2 Click;
        public Vector2 Disable;

        public Vector2 Get(AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Default:
                    return Default;
                case AnimationState.Enter:
                    return Enter;
                case AnimationState.Click:
                    return Click;
                case AnimationState.Disable:
                    return Disable;
                default:
                    goto case AnimationState.Default;
            }
        }
        public float Get_F(AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Default:
                    return 1 + (Default.x * 0.01f);
                case AnimationState.Enter:
                    return 1 + (Enter.x * 0.01f);
                case AnimationState.Click:
                    return 1 + (Click.x * 0.01f);
                case AnimationState.Disable:
                    return 1 + (Disable.x * 0.01f);
                default:
                    goto case AnimationState.Default;
            }
        }
    }

    [System.Serializable]
    public class AnimationSetVector3
    {
        public bool Use;
        public Vector3 Default;
        public Vector3 Enter;
        public Vector3 Click;
        public Vector3 Disable;

        public Vector3 Get(AnimationState state)
        {
            switch (state)
            {
                case AnimationState.Default:
                    return Default;
                case AnimationState.Enter:
                    return Enter;
                case AnimationState.Click:
                    return Click;
                case AnimationState.Disable:
                    return Disable;
                default:
                    goto case AnimationState.Default;
            }
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(IS_Selectable_Suppoter))]
    public class IS_Selectable_Suppoter_Editor : Editor
    {
        private IS_Selectable_Suppoter m_target;

        void OnEnable()
        {
            m_target = base.target as IS_Selectable_Suppoter;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Updater", GUILayout.Width(110));
            GUI.color = m_target.selectableRoot == null ? Color.red : Color.white;
            m_target.selectableRoot = (IS_Selectables)EditorGUILayout.ObjectField(m_target.selectableRoot, typeof(IS_Selectables), true);
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            if(m_target.selectableRoot != null)
            {
                float scale = 0;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Option");

                    EditorGUILayout.Space();
                    string addText = "";
                    if (m_target.lerpSpeed == 1)
                        addText = "[아주 느리다]";
                    else if (m_target.lerpSpeed < 5)
                        addText = "[느리다]";
                    else if (5 <= m_target.lerpSpeed && m_target.lerpSpeed < 10)
                        addText = "[조금 느리다]";
                    else if (10 <= m_target.lerpSpeed && m_target.lerpSpeed < 15)
                        addText = "[조금 빠르다]";
                    else if (15 <= m_target.lerpSpeed && m_target.lerpSpeed < 20)
                        addText = "[빠르다]";
                    else
                        addText = "[아주 빠르다]";
                    m_target.lerpSpeed = EditorGUILayout.Slider("Animation Speed  " + addText, m_target.lerpSpeed, 1f, 20f);
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    m_target.audios.Use = EditorGUILayout.Toggle(m_target.audios.Use, GUILayout.MaxWidth(12));
                    EditorGUILayout.LabelField(" Use Audio");
                    EditorGUILayout.EndHorizontal();

                    if (m_target.audios.Use)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            m_target.audios.audioMixer = (AudioMixerGroup)EditorGUILayout.ObjectField("AudioMixerGroup", m_target.audios.audioMixer, typeof(AudioMixerGroup), true);
                            m_target.audios.audioVolume = EditorGUILayout.Slider("Volume", m_target.audios.audioVolume, 0, 1);

                            m_target.audios.is3DAudio = EditorGUILayout.Toggle("3D Audio", m_target.audios.is3DAudio);
                            if (m_target.audios.is3DAudio)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.Separator();
                                    EditorGUILayout.BeginVertical();
                                    float min = EditorGUILayout.Slider("Min Distance", m_target.audios.soundDistance.Min, 0, 1);
                                    float max = EditorGUILayout.Slider("Max Distance", m_target.audios.soundDistance.Max, 0, 10);
                                    EditorGUILayout.EndVertical();

                                    if (max < min)
                                        max = min;

                                    m_target.audios.soundDistance = new MinMax(min, max);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            {
                                scale = EditorGUIUtility.currentViewWidth;
                                float splitWidth = (scale - 80) / 3;

                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Enter", GUILayout.MaxWidth(splitWidth));
                                m_target.audios.Enter = (AudioClip)EditorGUILayout.ObjectField("", m_target.audios.Enter, typeof(AudioClip), true, GUILayout.MaxWidth(splitWidth));
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Separator();

                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Exit", GUILayout.MaxWidth(splitWidth));
                                m_target.audios.Exit = (AudioClip)EditorGUILayout.ObjectField("", m_target.audios.Exit, typeof(AudioClip), true, GUILayout.MaxWidth(splitWidth));
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Separator();

                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Press", GUILayout.MaxWidth(splitWidth));
                                m_target.audios.Click = (AudioClip)EditorGUILayout.ObjectField("", m_target.audios.Click, typeof(AudioClip), true, GUILayout.MaxWidth(splitWidth));
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    if (GUILayout.Button("Get Text"))
                    {
                        IS_Func.GetComponentsInChildren(m_target.texts, m_target.transform);
                    }
                    if (m_target.texts == null || m_target.texts.Count == 0 || m_target.texts[0] == null)
                    {
                        IS_Func.GetComponentsInChildren<Text>(m_target.texts, m_target.transform);
                    }
                    if (m_target.texts != null && m_target.texts.Count != 0)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("Inner Texts");
                        for (int cnt = 0; cnt < m_target.texts.Count; cnt++)
                        {
                            if (m_target.texts[cnt] == null) break;

                            EditorGUILayout.BeginVertical();
                            {
                                scale = EditorGUIUtility.currentViewWidth;
                                float splitWidth = (scale - 60) / 6;
                                string parent = m_target.texts[cnt].transform.parent != null ? m_target.texts[cnt].transform.parent.name + "/" : string.Empty;
                                EditorGUILayout.LabelField(parent + m_target.texts[cnt].name, GUILayout.MaxWidth(splitWidth));

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Text", GUILayout.MaxWidth(splitWidth));
                                m_target.texts[cnt].text = EditorGUILayout.TextField(m_target.texts[cnt].text);
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Font Size", GUILayout.MaxWidth(splitWidth));
                                    m_target.texts[cnt].fontSize = EditorGUILayout.IntField(m_target.texts[cnt].fontSize);
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Font Color", GUILayout.MaxWidth(splitWidth));
                                    m_target.texts[cnt].color = EditorGUILayout.ColorField(m_target.texts[cnt].color);
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Animation List");
                    m_target.animationValues = ListController(m_target.animationValues);
                }
                EditorGUILayout.EndHorizontal();

                for (int cnt = 0; cnt < m_target.animationValues.Count; cnt++)
                {
                    if (m_target.animationValues[cnt] == null) return;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        scale = EditorGUIUtility.currentViewWidth;
                        float splitWidth = (scale - 220) / 4;

                        EditorGUILayout.BeginHorizontal();
                        {
                            ListController(m_target.animationValues, cnt, false);
                            if (m_target.animationValues[cnt] == null) return;

                            EditorGUILayout.LabelField($"[{cnt}]Target", GUILayout.MaxWidth(splitWidth));
                            m_target.animationValues[cnt].rect = (RectTransform)EditorGUILayout.ObjectField(m_target.animationValues[cnt].rect, typeof(RectTransform), true);

                            if (m_target.animationValues[cnt].rect != null &&
                                (m_target.animationValues[cnt].image == null ||
                                 m_target.animationValues[cnt].rect.transform != m_target.animationValues[cnt].image.transform))
                            {
                                m_target.animationValues[cnt].image = m_target.animationValues[cnt].rect.GetComponent<Image>();
                            }
                            if (m_target.animationValues[cnt].image)
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                m_target.animationValues[cnt].image = (Image)EditorGUILayout.ObjectField(m_target.animationValues[cnt].image, typeof(Image), true);
                                EditorGUI.EndDisabledGroup();
                            }

                            if (m_target.animationValues[cnt].rect != null &&
                               (m_target.animationValues[cnt].graphic == null ||
                                m_target.animationValues[cnt].rect.transform != m_target.animationValues[cnt].graphic.transform))
                            {
                                m_target.animationValues[cnt].graphic = m_target.animationValues[cnt].rect.GetComponent<Graphic>();
                            }
                            if (m_target.animationValues[cnt].graphic)
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                m_target.animationValues[cnt].graphic = (Graphic)EditorGUILayout.ObjectField(m_target.animationValues[cnt].graphic, typeof(Graphic), true);
                                EditorGUI.EndDisabledGroup();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (m_target.animationValues[cnt].rect != null)
                        {
                            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            {
                                EditorGUILayout.BeginHorizontal();
                                m_target.animationValues[cnt].Position.Use = EditorGUILayout.Toggle(m_target.animationValues[cnt].Position.Use, GUILayout.MaxWidth(12));
                                EditorGUILayout.LabelField("Position", GUILayout.MaxWidth(splitWidth));
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    m_target.animationValues[cnt].Sclale.Use = EditorGUILayout.Toggle(m_target.animationValues[cnt].Sclale.Use, GUILayout.MaxWidth(12));
                                    EditorGUILayout.LabelField("Sclale", GUILayout.MaxWidth(splitWidth));
                                }
                                EditorGUILayout.EndHorizontal();

                                if (m_target.animationValues[cnt].image)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    m_target.animationValues[cnt].Sprite.Use = EditorGUILayout.Toggle(m_target.animationValues[cnt].Sprite.Use, GUILayout.MaxWidth(12));
                                    EditorGUILayout.LabelField("Sprite", GUILayout.MaxWidth(splitWidth));
                                    EditorGUILayout.EndHorizontal();
                                }
                                if (m_target.animationValues[cnt].graphic)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    m_target.animationValues[cnt].Color.Use = EditorGUILayout.Toggle(m_target.animationValues[cnt].Color.Use, GUILayout.MaxWidth(12));
                                    EditorGUILayout.LabelField("Color", GUILayout.MaxWidth(splitWidth));
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.LabelField("Use Change", GUILayout.MaxWidth(125));
                                    if (m_target.animationValues[cnt].Position.Use)
                                        EditorGUILayout.LabelField("Position", GUILayout.MaxWidth(125));
                                    if (m_target.animationValues[cnt].Sclale.Use)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Sclale", GUILayout.MaxWidth(125 - 63));
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            EditorGUILayout.LabelField("[", GUILayout.MaxWidth(5));
                                            m_target.animationValues[cnt].Sclale.percent = EditorGUILayout.Toggle(m_target.animationValues[cnt].Sclale.percent, GUILayout.MaxWidth(11));

                                            if (m_target.animationValues[cnt].Sclale.percent)
                                            {
                                                EditorGUILayout.LabelField("%,", GUILayout.MaxWidth(14));
                                                m_target.animationValues[cnt].Sclale.localScale = EditorGUILayout.Toggle(m_target.animationValues[cnt].Sclale.localScale, GUILayout.MaxWidth(11));
                                                EditorGUILayout.LabelField("LC]", GUILayout.MaxWidth(22));
                                            }
                                            else
                                                EditorGUILayout.LabelField("%]", GUILayout.MaxWidth(47));
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    if (m_target.animationValues[cnt].image)
                                    {
                                        if (m_target.animationValues[cnt].Sprite.Use)
                                            EditorGUILayout.LabelField("Sprite", GUILayout.MaxWidth(125));
                                    }
                                    if (m_target.animationValues[cnt].graphic)
                                    {
                                        if (m_target.animationValues[cnt].Color.Use)
                                            EditorGUILayout.LabelField("Color", GUILayout.MaxWidth(125));
                                    }
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Separator();

                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Default", GUILayout.MaxWidth(splitWidth));
                                if (m_target.animationValues[cnt].Position.Use)
                                    m_target.animationValues[cnt].Position.Default = EditorGUILayout.Vector3Field("", m_target.animationValues[cnt].Position.Default, GUILayout.MaxWidth(splitWidth));
                                else
                                {
                                    m_target.animationValues[cnt].Position.Default = new Vector3();
                                }
                                if (m_target.animationValues[cnt].Sclale.Use)
                                {
                                    if (m_target.animationValues[cnt].Sclale.percent)
                                    {
                                        m_target.animationValues[cnt].Sclale.Default = new Vector2(EditorGUILayout.Slider("", m_target.animationValues[cnt].Sclale.Default.x, -100, 100, GUILayout.MaxWidth(splitWidth)), 0);
                                    }
                                    else
                                        m_target.animationValues[cnt].Sclale.Default = EditorGUILayout.Vector2Field("", m_target.animationValues[cnt].Sclale.Default, GUILayout.MaxWidth(splitWidth));
                                }
                                else
                                {
                                    m_target.animationValues[cnt].Sclale.Default = new Vector2();
                                }

                                if (m_target.animationValues[cnt].image)
                                {
                                    if (m_target.animationValues[cnt].Sprite.Use)
                                    {
                                        m_target.animationValues[cnt].Sprite.Default = (Sprite)EditorGUILayout.ObjectField(m_target.animationValues[cnt].Sprite.Default, typeof(Sprite), true, GUILayout.MaxWidth(splitWidth));
                                        if (m_target.animationValues[cnt].Sprite.Default != null &&
                                            m_target.animationValues[cnt].image != null &&
                                            m_target.animationValues[cnt].image.sprite != m_target.animationValues[cnt].Sprite.Default)
                                            m_target.animationValues[cnt].image.sprite = m_target.animationValues[cnt].Sprite.Default;
                                    }
                                    else
                                    {
                                        m_target.animationValues[cnt].Sprite.Default = null;
                                    }
                                }
                                if (m_target.animationValues[cnt].graphic)
                                {
                                    if (m_target.animationValues[cnt].Color.Use)
                                        m_target.animationValues[cnt].Color.Default = EditorGUILayout.ColorField(m_target.animationValues[cnt].Color.Default, GUILayout.MaxWidth(splitWidth));
                                    else
                                    {
                                        m_target.animationValues[cnt].Color.Default = Color.white;
                                    }
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Separator();

                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Enter", GUILayout.MaxWidth(splitWidth));
                                if (m_target.animationValues[cnt].Position.Use)
                                    m_target.animationValues[cnt].Position.Enter = EditorGUILayout.Vector3Field("", m_target.animationValues[cnt].Position.Enter, GUILayout.MaxWidth(splitWidth));
                                else
                                {
                                    m_target.animationValues[cnt].Position.Enter = new Vector3();
                                }
                                if (m_target.animationValues[cnt].Sclale.Use)
                                {
                                    if (m_target.animationValues[cnt].Sclale.percent)
                                    {
                                        m_target.animationValues[cnt].Sclale.Enter = new Vector2(EditorGUILayout.Slider("", m_target.animationValues[cnt].Sclale.Enter.x, -100, 100, GUILayout.MaxWidth(splitWidth)), 0);
                                    }
                                    else
                                        m_target.animationValues[cnt].Sclale.Enter = EditorGUILayout.Vector2Field("", m_target.animationValues[cnt].Sclale.Enter, GUILayout.MaxWidth(splitWidth));
                                }
                                else
                                {
                                    m_target.animationValues[cnt].Sclale.Enter = new Vector2();
                                }

                                if (m_target.animationValues[cnt].image)
                                {
                                    if (m_target.animationValues[cnt].Sprite.Use)
                                        m_target.animationValues[cnt].Sprite.Enter = (Sprite)EditorGUILayout.ObjectField(m_target.animationValues[cnt].Sprite.Enter, typeof(Sprite), true, GUILayout.MaxWidth(splitWidth));
                                    else
                                    {
                                        m_target.animationValues[cnt].Sprite.Enter = null;
                                    }
                                }
                                if (m_target.animationValues[cnt].graphic)
                                {
                                    if (m_target.animationValues[cnt].Color.Use)
                                        m_target.animationValues[cnt].Color.Enter = EditorGUILayout.ColorField(m_target.animationValues[cnt].Color.Enter, GUILayout.MaxWidth(splitWidth));
                                    else
                                    {
                                        m_target.animationValues[cnt].Color.Enter = Color.white;
                                    }
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Separator();

                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Press", GUILayout.MaxWidth(splitWidth));
                                if (m_target.animationValues[cnt].Position.Use)
                                    m_target.animationValues[cnt].Position.Click = EditorGUILayout.Vector3Field("", m_target.animationValues[cnt].Position.Click, GUILayout.MaxWidth(splitWidth));
                                else
                                {
                                    m_target.animationValues[cnt].Position.Click = new Vector3();
                                }
                                if (m_target.animationValues[cnt].Sclale.Use)
                                {
                                    if (m_target.animationValues[cnt].Sclale.percent)
                                    {
                                        m_target.animationValues[cnt].Sclale.Click = new Vector2(EditorGUILayout.Slider("", m_target.animationValues[cnt].Sclale.Click.x, -100, 100, GUILayout.MaxWidth(splitWidth)), 0);
                                    }
                                    else
                                        m_target.animationValues[cnt].Sclale.Click = EditorGUILayout.Vector2Field("", m_target.animationValues[cnt].Sclale.Click, GUILayout.MaxWidth(splitWidth));
                                }
                                else
                                {
                                    m_target.animationValues[cnt].Sclale.Click = new Vector2();
                                }
                                if (m_target.animationValues[cnt].image)
                                {
                                    if (m_target.animationValues[cnt].Sprite.Use)
                                        m_target.animationValues[cnt].Sprite.Click = (Sprite)EditorGUILayout.ObjectField(m_target.animationValues[cnt].Sprite.Click, typeof(Sprite), true, GUILayout.MaxWidth(splitWidth));
                                    else
                                    {
                                        m_target.animationValues[cnt].Sprite.Click = null;
                                    }
                                }
                                if (m_target.animationValues[cnt].graphic)
                                {
                                    if (m_target.animationValues[cnt].Color.Use)
                                        m_target.animationValues[cnt].Color.Click = EditorGUILayout.ColorField(m_target.animationValues[cnt].Color.Click, GUILayout.MaxWidth(splitWidth));
                                    else
                                    {
                                        m_target.animationValues[cnt].Color.Click = Color.white;
                                    }
                                }
                                EditorGUILayout.EndVertical();

                                EditorGUILayout.Separator();

                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.LabelField("Disable", GUILayout.MaxWidth(splitWidth));
                                if (m_target.animationValues[cnt].Position.Use)
                                    m_target.animationValues[cnt].Position.Disable = EditorGUILayout.Vector3Field("", m_target.animationValues[cnt].Position.Disable, GUILayout.MaxWidth(splitWidth));
                                else
                                {
                                    m_target.animationValues[cnt].Position.Disable = new Vector3();
                                }
                                if (m_target.animationValues[cnt].Sclale.Use)
                                {
                                    if (m_target.animationValues[cnt].Sclale.percent)
                                    {
                                        m_target.animationValues[cnt].Sclale.Disable = new Vector2(EditorGUILayout.Slider("", m_target.animationValues[cnt].Sclale.Disable.x, -100, 100, GUILayout.MaxWidth(splitWidth)), 0);

                                    }
                                    else
                                        m_target.animationValues[cnt].Sclale.Disable = EditorGUILayout.Vector2Field("", m_target.animationValues[cnt].Sclale.Disable, GUILayout.MaxWidth(splitWidth));
                                }
                                else
                                {
                                    m_target.animationValues[cnt].Sclale.Disable = new Vector2();
                                }
                                if (m_target.animationValues[cnt].image)
                                {
                                    if (m_target.animationValues[cnt].Sprite.Use)
                                        m_target.animationValues[cnt].Sprite.Disable = (Sprite)EditorGUILayout.ObjectField(m_target.animationValues[cnt].Sprite.Disable, typeof(Sprite), true, GUILayout.MaxWidth(splitWidth));
                                    else
                                    {
                                        m_target.animationValues[cnt].Sprite.Disable = null;
                                    }
                                }
                                if (m_target.animationValues[cnt].graphic)
                                {
                                    if (m_target.animationValues[cnt].Color.Use)
                                        m_target.animationValues[cnt].Color.Disable = EditorGUILayout.ColorField(m_target.animationValues[cnt].Color.Disable, GUILayout.MaxWidth(splitWidth));
                                    else
                                    {
                                        m_target.animationValues[cnt].Color.Disable = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                                    }
                                }
                                EditorGUILayout.EndVertical();

                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            //여기까지 검사해서 필드에 변화가 있으면
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
                EditorUtility.SetDirty(m_target);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private List<T> ListController<T>(List<T> list, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(120));
            {
                EditorGUILayout.LabelField($"Total[{list.Count}]", GUILayout.MaxWidth(60));


                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    if (list.Count != 0)
                    {
                        if (EditorUtility.DisplayDialog("경고", "초기화 할거임?\n복구 못해", "응", "아니"))
                            list = new List<T>();
                    }
                    else
                        EditorUtility.DisplayDialog("이런", "초기화 할게 없져", "응");
                }
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    list.Add(default);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "갯수 줄일거임?\n줄이면 값 넣은거 사라져", "응", "아니"))
                    {
                        if (list.Count != 0)
                            list.RemoveAt(list.Count - 1);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
        private List<T> ListController<T>(List<T> list, int num, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();
            if (list.Count == 0)
                list.Add(default);

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(60));
            {
                if (GUILayout.Button("D", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "삭제 할거임?\n복구 못해", "응", "아니"))
                        list.RemoveAt(num);
                }

                EditorGUI.BeginDisabledGroup(!(0 < num));
                {
                    if (GUILayout.Button("△", GUILayout.Width(20)))
                    {
                        list.Insert(num - 1, list[num]);
                        list.RemoveAt(num + 1);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!(num < list.Count - 1));
                {
                    if (GUILayout.Button("▽", GUILayout.Width(20)))
                    {
                        list.Insert(num + 2, list[num]);
                        list.RemoveAt(num);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }
    }
#endif
}