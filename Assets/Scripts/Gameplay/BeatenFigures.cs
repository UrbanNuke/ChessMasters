using System.Collections;
using System.Collections.Generic;
using Misc;
using UnityEngine;

namespace Gameplay
{
    public class BeatenFigures : MonoBehaviour
    {
        [SerializeField]
        private Transform whiteField;
        [SerializeField]
        private Transform blackField;
        
        private readonly List<Figure> _whiteFigures = new List<Figure>(16);
        private readonly List<Figure> _blackFigures = new List<Figure>(16);
        private const int HalfOfField = 8;

        public void PutBeatenFigure(Figure figure)
        {
            if (figure.Color == FigureColor.White)
                _whiteFigures.Add(figure);
            else
                _blackFigures.Add(figure);
                
            StartCoroutine(MoveFigureToBeatenField(figure.Color));
        }

        private IEnumerator MoveFigureToBeatenField(FigureColor color)
        {
            float t = 0;
            
            int index = color == FigureColor.White ? _whiteFigures.Count - 1 : _blackFigures.Count - 1;
            Figure figure = color == FigureColor.White ? _whiteFigures[index] : _blackFigures[index];
            
            Vector3 firstBezierPoint = figure.transform.position;

            Transform beatenField = color == FigureColor.White ? whiteField : blackField;
            Vector3 thirdBezierPoint = index <= BoardService.Border
                    ? beatenField.position + (beatenField.right * (index * BoardService.FieldOffset))
                    : (beatenField.position + beatenField.forward * -0.5f) + (beatenField.right * ((index - HalfOfField) * BoardService.FieldOffset));
            
            Vector3 distance = thirdBezierPoint - firstBezierPoint;
            Vector3 secondBezierPoint = firstBezierPoint + distance / 4;
            secondBezierPoint.y = 1.5f;

            while (t < 1f)
            {
                Vector3 a = Vector3.Lerp(firstBezierPoint, secondBezierPoint, t);
                Vector3 b = Vector3.Lerp(secondBezierPoint, thirdBezierPoint, t);
                figure.transform.position = Vector3.Lerp(a, b, t);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}