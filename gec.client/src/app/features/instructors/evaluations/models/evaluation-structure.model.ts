import {DescriptorModel} from './descriptor.model';

export interface EvaluationStructureModel {
  id: string;
  key: string;
  name: string;
  description: string;
  type: string; // Ejemplo: "Competency" o "Subcompetency"
  parentId: string | null;
  parentName: string | null;
  descriptors: DescriptorModel[];
}
