using UnityEngine;
using TMPro;

public class TrackButtonController : MonoBehaviour
{
    private string trackTitle;

    [SerializeField] private TMP_Text textTitle;

    /// <summary>
    /// Initializes the track button with the track title.
    /// </summary>
    /// <param name="trackTitle">The title of the track to display.</param>
    public void Initialize(string trackTitle)
    {
        this.trackTitle = trackTitle;
        UpdateTitle(trackTitle);
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
        if (!string.IsNullOrEmpty(trackTitle))
        {
            Mp3PlayerController.Instance.PlayTrackByTitle(trackTitle);
        }
        else
        {
            Debug.LogWarning("Track title is null or empty.");
        }
    }

    /// <summary>
    /// Highlights or unhighlights the button text based on the state.
    /// </summary>
    /// <param name="state">True to highlight, false to unhighlight.</param>
    public void HighlightButton(bool state)
    {
        if (textTitle == null) return;

        textTitle.text = state
            ? $"<b>{trackTitle}</b>"
            : trackTitle;
    }
}
