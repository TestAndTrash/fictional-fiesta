
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class OpponentHandManager : MonoBehaviour
{
    private List<GameObject> handCards = new();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject cardBackPrefab;

    public float spacing;
    public void DrawCard()
    {
        GameObject cardObject = Instantiate(cardBackPrefab, spawnPoint.position, spawnPoint.rotation);
        handCards.Add(cardObject);
        UpdateCardPos();
    }

    private void UpdateCardPos()
    {
        if (handCards.Count == 0) return;
        float cardSpacing = 1f / spacing;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            float position = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(position);
            Vector3 splinePositionWorld = splineContainer.transform.TransformPoint(splinePosition);
            Vector3 forward = spline.EvaluateTangent(position);
            Vector3 up = spline.EvaluateUpVector(position);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
            handCards[i].transform.DOMove(splinePositionWorld, 0.25f);
        }
    }

    public void DeleteACardFromHand()
    {
        //debug plus tard mamene
        //handCards.RemoveRange(0, 1);
        UpdateCardPos();
    }
}