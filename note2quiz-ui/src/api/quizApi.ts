import { API_ENDPOINTS } from "@/config/apiConfig";

export enum Difficulty {
	Easy = "Easy",
	Medium = "Medium",
	Hard = "Hard"
}

export interface OptionDto {
	optionId: number;
	optionText: string;
	isCorrect?: boolean;
}

export interface QuestionDto {
	id: number;
	text: string;
	options: OptionDto[];
}

export interface QuizResponse {
	quizSessionId: number;
	questions: QuestionDto[];
}

export interface AnswerDto {
	questionId: number;
	selectedOptionId: number;
}

export interface SubmitQuizRequest {
	quizSessionId: number;
	answers: AnswerDto[];
}

export interface QuestionResultDto {
	questionId: number;
	selectedOptionId: number;
	correctOptionId: number;
	isCorrect: boolean;
}

export interface SubmitQuizResponse {
	quizSessionId: number;
	score: number;
	totalQuestions: number;
	results: QuestionResultDto[];
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
export const quizSession = async (id:number,token:string) =>{
    const newToken = token.trim();
    const response = await fetch(API_ENDPOINTS.quiz.quizSession(id),{
        method:"GET",
        headers:{
            Authorization: `Bearer ${newToken}`
        }
    });
    if(!response.ok) throw new Error("Failed to get quiz Session");
    return response.json();
}
