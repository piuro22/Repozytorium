using UnityEngine;
using TMPro;

public class TrackButtonController : MonoBehaviour
{
    private Mp3TrackProperties mp3TrackProperties;

    [SerializeField] private TMP_Text textTitle;

    /// <summary>
    /// Initializes the track button with track properties.
    /// </summary>
    /// <param name="mp3TrackProperties">Properties of the track.</param>
    public void Initialize(Mp3TrackProperties mp3TrackProperties)
    {
        this.mp3TrackProperties = mp3TrackProperties;
        UpdateTitle(mp3TrackProperties.title);
    }

    /// <summary>
    /// Updates the title text of the button.
    /// </summary>
    /// <param name="title">Title to display on the button.</param>
    private void UpdateTitle(string title)
    {
        if (textTitle != null)
        {
            textTitle.SetText(title);
        }
    }

    /// <summary>
    /// Handles the button click event to play the associated track.
    /// </summary>
    public void OnButtonClick()
    {
        if (mp3TrackProperties != null)
        {
            Mp3PlayerController.Instance.PlayTrackByTitle(mp3TrackProperties.title);
        }
    }

    /// <summary>
    /// Highlights or unhighlights the button text based on the state.
    /// </summary>
    /// <param name="state">True to highlight, false to unhighlight.</param>
    public void HighLightButton(bool state)
    {
        if (textTitle != null)
        {
            if (state)
            {
                textTitle.text = $"<b>{mp3TrackProperties.title}</b>";
            }
            else
            {
                textTitle.text = mp3TrackProperties.title;
            }
        }
    }
}
