using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Modal : MonoBehaviour
{
    public string Title;
    public string Body;
    private GameObject content;
    private Text titleText;
    private Text bodyText;
    private Action onClose;
    void Start()
    {
        content = transform.Find("Content").gameObject;
        titleText = content.transform.Find("Title").GetComponent<Text>();
        bodyText = content.transform.Find("Body").GetComponent<Text>();
        content.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(()=> {
            if (onClose != null)
            {
                onClose();
            }
            transform.parent.SendMessageUpwards("modalClosed");
            content.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        titleText.text = Title;
        bodyText.text = Body;
    }

    public void UpdateModal(ModalValues values)
    {
        Title = values.Title != null ? values.Title : Title;
        Body = values.Body != null ? values.Body : Body;
        onClose = values.onClose;
        content.SetActive(values.Visible);
    }

    public class ModalValues
    {
        public readonly string Title;
        public readonly string Body;
        public readonly bool Visible;
        public readonly Action onClose;

        public ModalValues(string title, string body, bool visible, Action onClose)
        {
            this.Title = title;
            this.Body = body;
            this.Visible = visible;
            this.onClose = onClose;
        }

        public ModalValues(string title, string body, bool visible) : this(title, body, visible, null) { }
    }
}
