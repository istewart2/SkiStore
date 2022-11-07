import { Button } from '@mui/material';
import { Product } from '../../app/models/Product';
import ProductList from './ProductList';

interface Props {
  products: Product[];
  addProduct: () => void;
}

export default function Catalog({ products, addProduct }: Props) {
  return (
    <>
      <ProductList products={products}></ProductList>
      <Button variant='contained' onClick={addProduct}>
        Add product
      </Button>
    </>
  );
}
