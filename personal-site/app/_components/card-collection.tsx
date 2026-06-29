import type { ReactNode } from "react";
import { EmptyState, ErrorState } from "@/app/_components/site-shell";
import type { ApiState } from "@/app/_lib/api";

export function CardCollection<T>({
  state,
  limit,
  emptyTitle,
  children,
  renderItem,
}: {
  state: ApiState<T[]>;
  limit?: number;
  emptyTitle: string;
  children: ReactNode;
  renderItem: (item: T) => ReactNode;
}) {
  if (!state.ok) {
    return <ErrorState message={state.message} />;
  }

  const items =
    typeof limit === "number" ? state.data.slice(0, limit) : state.data;

  if (!items.length) {
    return <EmptyState title={emptyTitle}>{children}</EmptyState>;
  }

  return <div className="card-grid">{items.map(renderItem)}</div>;
}
