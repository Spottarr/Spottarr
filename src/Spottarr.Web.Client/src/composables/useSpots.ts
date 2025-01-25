import { ref } from 'vue';
import type { Ref } from 'vue';
import type { SpotTableRowResponseDto } from '@/types/generated/spot-table-row-response-dto';
import reviver from '@/helpers/reviver';

const spots: Ref<SpotTableRowResponseDto[]> = ref([]);
const loading: Ref<boolean> = ref(false);
const error: Ref<Error | null> = ref(null);
const apiHost = import.meta.env.VITE_API_HOST;

const fetchSpots = async () => {
  loading.value = true;
  error.value = null;
  try {
    const response = await fetch(`${apiHost}/api/spots`);
    const body = await response.text();
    spots.value = JSON.parse(body, reviver);
  } catch (e) {
    if (e instanceof Error) error.value = e;
  } finally {
    loading.value = false;
  }
};

const nzbUrl = (spotId: number) => `${apiHost}/api/spots/${spotId}/nzb`;

export default function useSpots() {
  return {
    spots,
    loading,
    error,
    fetchSpots,
    nzbUrl,
  };
}
