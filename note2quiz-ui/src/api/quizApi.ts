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
	difficulty?: Difficulty;
}

export const uploadImageAndGenerateQuiz = async (
	file: File,
	difficulty: Difficulty,
	token: string
): Promise<QuizResponse> => {
	const formData = new FormData();
	formData.append("file", file);
	formData.append("difficulty", difficulty);

	const response = await fetch(API_ENDPOINTS.quiz.generate, {
		method: "POST",
		headers: {
			Authorization: `Bearer ${token.trim()}`
		},
		body: formData
	});

	if (!response.ok) {
		throw new Error("Failed to generate quiz");
	}

	return response.json();
};

export const quizHistory = async (token: string): Promise<QuizHistory[]> => {
	const response = await fetch(API_ENDPOINTS.quiz.history, {
		method: "GET",
		headers: {
			Authorization: `Bearer ${token.trim()}`
		}
	});

	if (!response.ok) {
		throw new Error("Failed to get quiz history");
	}

	return response.json();
};

export const quizSession = async (
	id: number,
	token: string
): Promise<QuizResponse> => {
	const response = await fetch(API_ENDPOINTS.quiz.quizSession(id), {
		method: "GET",
		headers: {
			Authorization: `Bearer ${token.trim()}`
		}
	});

	if (!response.ok) {
		throw new Error("Failed to get quiz session");
	}

	return response.json();
};

export const submitQuiz = async (
	payload: SubmitQuizRequest,
	token: string
): Promise<SubmitQuizResponse> => {
	const response = await fetch(API_ENDPOINTS.quiz.submit, {
		method: "POST",
		headers: {
			"Content-Type": "application/json",
			Authorization: `Bearer ${token.trim()}`
		},
		body: JSON.stringify(payload)
	});

	if (!response.ok) {
		throw new Error("Failed to submit quiz");
	}

	return response.json();
};