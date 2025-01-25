import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '@/views/HomeView.vue';
import NotFoundView from "@/views/NotFoundView.vue";
import SpotsView from "@/views/SpotsView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/spots',
      name: 'spots',
      component: SpotsView,
    },
    {
      path: '/:pathMatch(.*)',
      name: 'not-found',
      component: NotFoundView,
    },
  ],
});

export default router;
