import { API_ENDPOINTS } from "@/config/apiConfig";

export enum Difficulty {
	Easy = "Easy",
	Medium = "Medium",
	Hard = "Hard"
}
export const uploadImageAndGenerateQuiz = async(
    file:File,
    difficulty: Difficulty,
    token:string
) =>
{
    const formData = new FormData();
	formData.append("file", file);
	formData.append("difficulty", difficulty);
    const newToken = token.trim();
	const response = await fetch(API_ENDPOINTS.quiz.generate, {
		method: "POST",
		headers: {
			Authorization: `Bearer ${newToken}`
		},
		body: formData
	});
	if (!response.ok) throw new Error("Failed to generate quiz");
	return response.json();
}

export interface QuizHistory {
	quizSessionId: number;
	createdAt: string;
	questionCount: number;
	score?: number;
	difficulty: Difficulty;
}

export const quizHistory = async (token: string): Promise<QuizHistory[]> => {
    const newToken = token.trim();
    console.log("toke.....",newToken);
	const response = await fetch(API_ENDPOINTS.quiz.history, {
		method: "GET",
		headers: {
			Authorization: `Bearer ${newToken}`
		}
	});
	if (!response.ok) throw new Error("Failed to get quiz history");
	return response.json();
};
