export interface Event {
  id: number;
  name: string;
  description: string;
  startDate: Date;
  eventPlace: string;
  category: string;
  maxParticipantsCount: number;
  isFull: boolean;
  imagePath?: string;
}