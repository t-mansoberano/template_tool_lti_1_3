export function calculateRubricScore(scores: number[]): number {
  if (!scores || scores.length === 0) {
    return 0;
  }
  return scores.reduce((sum, score) => sum + score, 0) / scores.length;
}
