import { UserRole } from '../models/enums/UserRole'

const ROLE_STORAGE_KEY = 'comicrealm.userRole'

export const roleLabels: Record<UserRole, string> = {
  [UserRole.SuperAdmin]: 'SuperAdmin',
  [UserRole.Admin]: 'Admin',
  [UserRole.Friend]: 'Friend',
}

export function getCurrentUserRole(): UserRole | null {
  const rawRole = localStorage.getItem(ROLE_STORAGE_KEY)

  if (rawRole === null) {
    return null
  }

  const parsedRole = Number.parseInt(rawRole, 10)

  if (parsedRole === UserRole.SuperAdmin || parsedRole === UserRole.Admin || parsedRole === UserRole.Friend) {
    return parsedRole
  }

  return null
}

export function setCurrentUserRole(role: UserRole): void {
  localStorage.setItem(ROLE_STORAGE_KEY, String(role))
}

export function clearCurrentUserRole(): void {
  localStorage.removeItem(ROLE_STORAGE_KEY)
}

export function isAuthenticated(role: UserRole | null): boolean {
  return role !== null
}

export function getCreatableRolesByActor(actorRole: UserRole | null): UserRole[] {
  if (actorRole === UserRole.SuperAdmin) {
    return [UserRole.Admin]
  }

  if (actorRole === UserRole.Admin) {
    return [UserRole.Admin, UserRole.Friend]
  }

  return []
}

export function canAccessCreateUserPage(role: UserRole | null): boolean {
  return role === UserRole.SuperAdmin || role === UserRole.Admin
}

export function canViewComics(role: UserRole | null): boolean {
  return role === UserRole.SuperAdmin || role === UserRole.Admin || role === UserRole.Friend
}

export function canManageComics(role: UserRole | null): boolean {
  return role === UserRole.Admin
}

export function canCreateRole(actorRole: UserRole | null, targetRole: UserRole): boolean {
  return getCreatableRolesByActor(actorRole).includes(targetRole)
}
