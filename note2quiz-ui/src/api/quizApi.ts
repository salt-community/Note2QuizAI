import { API_ENDPOINTS } from "@/config/apiConfig";

export enum Difficulty {
  Easy = "Easy",
  Medium = "Medium",
  Hard = "Hard"
}
export const uploadImageAndGenerateQuiz = async(
    file:File,
     difficulty: Difficulty
) =>
{
    const formData = new FormData();
    formData.append("image",file);
    formData.append("difficulty",difficulty);
    const response = await fetch(
       API_ENDPOINTS.quiz.generate
        ,{
            method:"POST",
            body:formData,
        }
    );
    if(!response.ok)
        throw new Error("Failed to generate quiz");
    return response.json();
}