const BASE_URL = import.meta.env.VITE_API_BASE_URL;
if (!BASE_URL) throw new Error("Base URL not defined!");

export const API_ENDPOINTS = {
	quiz: {
		generate: `${BASE_URL}/api/quiz`,
		history: `${BASE_URL}/api/quiz/history`,
        quizSession:(id:number)=>`${BASE_URL}/api/quiz/${id}`
	}
};
