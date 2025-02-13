export interface DescriptorModel {
  id: string;
  level: 'Destacado' | 'Sólido' | 'Básico' | 'Incipiente' | 'NoElementosSuficientes';
  description: string;
}
